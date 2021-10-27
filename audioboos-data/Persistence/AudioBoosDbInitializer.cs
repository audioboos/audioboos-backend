using Microsoft.AspNetCore.Identity;

namespace AudioBoos.Data.Persistence; 

public static class AudioBoosDbInitializer {
    public static void SeedUsers<TUser>(UserManager<TUser> userManager) where TUser : IdentityUser, new() {
        if (userManager.FindByEmailAsync("fergal.moran+audioboos@gmail.com").Result != null) {
            return;
        }

        var user = new TUser {
            UserName = "fergal.moran+audioboos@gmail.com",
            Email = "fergal.moran+audioboos@gmail.com"
        };

        IdentityResult result = userManager.CreateAsync(user, "secret").Result;

        if (result.Succeeded) {
            userManager.AddToRoleAsync(user, "Admin").Wait();
        }
    }
}