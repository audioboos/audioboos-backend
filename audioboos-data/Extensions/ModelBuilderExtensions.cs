using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AudioBoos.Data.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AudioBoos.Data.Extensions;

public static class ModelBuilderExtensions {
    private static IEnumerable<UniqueKeyAttribute> _getUniqueKeyAttributes(this IMutableEntityType entityType,
        IMutableProperty property) {
        if (entityType == null) {
            throw new ArgumentNullException(nameof(entityType));
        }

        if (entityType.ClrType == null) {
            throw new ArgumentNullException(nameof(entityType.ClrType));
        }

        if (property == null) {
            throw new ArgumentNullException(nameof(property));
        }

        if (property.Name == null) {
            throw new ArgumentNullException(nameof(property.Name));
        }

        var propInfo = entityType.ClrType.GetProperty(
            property.Name,
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.Instance |
            BindingFlags.DeclaredOnly);
        if (propInfo == null) {
            return null;
        }

        return propInfo.GetCustomAttributes<UniqueKeyAttribute>();
    }

    public static void CreateUniqueKeys(this ModelBuilder builder) {
        foreach (var entityType in builder.Model.GetEntityTypes()) {
            var properties = entityType.GetProperties();
            var enumerable = properties as IMutableProperty[] ?? properties.ToArray();
            if ((!enumerable.Any())) {
                continue;
            }

            foreach (var property in enumerable) {
                var uniqueKeys = entityType._getUniqueKeyAttributes(property);
                if (uniqueKeys == null) {
                    continue;
                }

                foreach (var uniqueKey in uniqueKeys.Where(x => x.Order == 0)) {
                    // Single column Unique Key
                    if (String.IsNullOrWhiteSpace(uniqueKey.GroupId)) {
                        entityType.AddIndex(property).IsUnique = true;
                    }
                    // Multiple column Unique Key
                    else {
                        var mutableProperties = new List<IMutableProperty>();
                        enumerable.ToList().ForEach(x => {
                            var uks = entityType._getUniqueKeyAttributes(x);
                            if (uks == null) {
                                return;
                            }

                            mutableProperties.AddRange(from uk in uks
                                where (uk != null) && (uk.GroupId == uniqueKey.GroupId) select x);
                        });
                        entityType.AddIndex(mutableProperties).IsUnique = true;
                    }
                }
            }
        }
    }

    public static void SeedRoles(this ModelBuilder builder) {
        var adminRoleId = Guid.Parse("b8e58349-3353-4aac-91a2-7146bb1a64cb").ToString();
        var editorRoleId = Guid.Parse("ad46f0b2-8ced-4786-93af-0306ba77d522").ToString();
        var viewerRoleId = Guid.Parse("0f50900a-19dd-48d5-9aa9-e2f298ef4ef1").ToString();

        var adminConcurrencyStamp = Guid.Parse("0bd4d47e-8d86-4d75-a3ca-8dc175af4564").ToString();
        var editorConcurrencyStamp = Guid.Parse("014638e8-fcff-4f9d-bc60-ce6ed0250965").ToString();
        var viewerConcurrencyStamp = Guid.Parse("4db8a34e-3084-4f5d-abdb-9da2265b824f").ToString();

        builder.Entity<IdentityRole>().HasData(new List<IdentityRole> {
            new() {
                Id = adminRoleId,
                ConcurrencyStamp = adminConcurrencyStamp,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new() {
                Id = editorRoleId,
                ConcurrencyStamp = editorConcurrencyStamp,
                Name = "Editor",
                NormalizedName = "EDITOR"
            },
            new() {
                Id = viewerRoleId,
                ConcurrencyStamp = viewerConcurrencyStamp,
                Name = "Viewer",
                NormalizedName = "VIEWER"
            },
        });
    }
}
