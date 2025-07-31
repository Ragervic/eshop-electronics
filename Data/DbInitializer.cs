// using Microsoft.AspNetCore.Identity;
// using TestP.Models;

// namespace TestP.Data
// {
//     public static class DbInitializer
//     {
//         public static async Task Initialize(IServiceProvider serviceProvider)
//         {
//             using (var scope = serviceProvider.CreateScope())
//             {
//                 var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//                 var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//                 await EnsureRoleAsync(roleManager, UserRoles.Customer);
//                 await EnsureRoleAsync(roleManager, UserRoles.Admin);
//                 await EnsureRoleAsync(roleManager, UserRoles.Staff);

//                 if (await userManager.FindByNameAsync("admin@example.com") == null)
//                 {
//                     var adminUser = new ApplicationUser
//                     {
//                         UserName = "admin@example.com",
//                         Email = "admin@example.com",
//                         EmailConfirmed = true,
//                         FirstName = "Super",
//                         LastName = "Admin"
//                     };

//                     var result = await userManager.CreateAsync(adminUser, "AdminP@ssw0rd!");
//                     if (result.Succeeded)
//                     {
//                         await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
//                     }
//                     else
//                     {
//                         Console.WriteLine("Error creating Admin!!");
//                         foreach (var error in result.Errors)
//                         {
//                             Console.WriteLine($"- {error.Description}");
//                         }

//                     }
//                 }
//                 else
//                 {
//                     Console.WriteLine("Adminalready exists!!");
//                 }
//             }
//         }
//         private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
//         {
//             if (!await roleManager.RoleExistsAsync(roleName))
//             {
//                 await roleManager.CreateAsync(new IdentityRole(roleName));
//             }
//         }

//     }

// }
