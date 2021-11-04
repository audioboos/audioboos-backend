using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AudioBoos.Data.Persistence.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AudioBoos.Data.Persistence.Extensions; 

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
        var adminRoleId = Guid.NewGuid().ToString();
        var editorRoleId = Guid.NewGuid().ToString();
        var viewerRoleId = Guid.NewGuid().ToString();

        builder.Entity<IdentityRole>().HasData(new List<IdentityRole> {
            new() {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new() {
                Id = editorRoleId,
                Name = "Editor",
                NormalizedName = "EDITOR"
            },
            new() {
                Id = viewerRoleId,
                Name = "Viewer",
                NormalizedName = "VIEWER"
            },
        });
    }
}