using Antimicrobici.Core.Models;
using Antimicrobici.Core.Utils;
using Antimicrobici.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Services
{
    public interface IMenuService
    {
        MenuProfile GetMenuProfile();
        List<Menu> GetMenu();
        Menu GetSingoloMenu(string nameMenu);
        List<Menu> GetMenuPrimoLivello(string userName, out String landingPage);
    }

    public class MenuService : BaseService<Menu, long, AmDbContext>, IMenuService
    {

        private readonly ILogger<MenuService> logger;
        
        public MenuService(ILogger<MenuService> logger, AmDbContext ctx = null)
            : base(ctx) 
        {
            this.logger = logger;
        }
        
        public MenuProfile GetMenuProfile()
        {
            #region DECLARATION
            List<MenuEntry> webMenu = new List<MenuEntry>();
            List<MenuEntry> webFigli = new List<MenuEntry>();
            String landingPage = String.Empty;
            String userID = String.Empty;
            MenuProfile prf = new MenuProfile();
            #endregion

            try
            {
                // Authentication
                //userID = Authentication.GetWindowsAuthenticated();
                userID = "siamorellini";

                List<Menu> menus = GetMenuPrimoLivello(userID, out landingPage);
                //List<Menu> menus = new List<Menu>();
                prf.landingPage = landingPage;
                // Log.logInfo("Ho eseguito Query con EF");
                foreach (Menu menu in menus)
                {
                    MenuEntry mnu = new MenuEntry();
                    mnu.id = menu?.Id.ToString();
                    mnu.name = menu?.Name;
                    mnu.icon = menu?.Icon;

                    #region LOAD MENU E FIGLI
                    List<Menu> figli = GetFigli(menu.Id, userID);
                    if (figli != null && figli.Count > 0)
                    {
                        webFigli = new List<MenuEntry>();
                        foreach (Menu item in figli)
                        {
                            MenuEntry figlio = new MenuEntry();
                            figlio.id = item.Id.ToString();
                            figlio.name = item.Name;
                            figlio.icon = item.Icon;
                            webFigli.Add(figlio);
                        }
                        mnu.items = webFigli.ToArray();
                        webMenu.Add(mnu);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Errore generico EF", ex);
                // prf.landingPage = @"/f/forbidden";
                prf.landingPage = landingPage;
            }

            prf.menu = webMenu.ToArray();
            if (String.IsNullOrWhiteSpace(prf.landingPage))
            {
                prf.landingPage = @"richieste/farmacisti";
            }
            return prf;
        }

        public List<Menu> GetMenu()
        {
            List<Menu> result = null;
            result = ctx.Menu.OrderBy(x => x.Ordering).ToList();
            return result;
        }

        public Menu GetSingoloMenu(string nameMenu)
        {
            var result = ctx.Menu.Where(x => x.Name == nameMenu).FirstOrDefault();
            return result;
        }

        public List<Menu> GetMenuPrimoLivello(string userName, out String landingPage)
        {
            #region DECLARATION
            List<Menu> result = new List<Menu>();
            List<long> keys = new List<long>();
            String s = String.Empty;
            landingPage = string.Empty;
            #endregion

            var conn = ctx.Database.GetDbConnection();

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                
                using var command = conn.CreateCommand();
                var qb = new QueryBuilder();
                {
                    s = @" SELECT 
                            Principal.LandingPage,
                            Menu.Description, 
                            Menu.Id, Menu.Icon, Menu.Name                            
                            FROM Menu
                            INNER JOIN GroupMenu ON Menu.Id = GroupMenu.MenuId
                            INNER JOIN PrincipalGroup ON GroupMenu.GroupId = PrincipalGroup.GroupId
                            INNER JOIN Principal ON Principal.Id = PrincipalGroup.PrincipalId
                          WHERE Principal.Username = @username
                          AND Menu.ParentId IS NULL
                          AND GroupMenu.ToExec = 1
                          ORDER BY Menu.Ordering";
                    
                    command.CommandText = s;
                    command.Parameters.Add(new SqlParameter("username", userName));
                    //qb.Create(command);

                    using var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int i = 0;
                            Menu menu = new Menu();
                            landingPage = reader.GetNullableString(i++);
                            menu.Description = reader.GetNullableString(i++);
                            menu.Id = reader.GetInt64(i++);
                            menu.Icon = reader.GetNullableString(i++);
                            menu.Name = reader.GetNullableString(i++);
                            try
                            {
                                if (!keys.Contains(menu.Id))
                                {
                                    keys.Add(menu.Id);
                                    result.Add(menu);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                logger.LogError("errore in recupero menu", ex);
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }

        public List<Menu> GetFigli(long idPadre, string userName)
        {
            List<Menu> result = new List<Menu>();
            List<long> keys = new List<long>();
            String s = String.Empty;
            List<SqlParameter> paras = new List<SqlParameter>();

            var conn = ctx.Database.GetDbConnection();

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using var command = conn.CreateCommand();
                s = @" SELECT Menu.Id, Menu.Description, 
                    Menu.Icon, Menu.Name, 
                    Principal.LandingPage
                    FROM Menu
                    INNER JOIN GroupMenu ON Menu.Id = GroupMenu.MenuId
                    INNER JOIN PrincipalGroup ON GroupMenu.GroupId = PrincipalGroup.GroupId
                    INNER JOIN Principal ON Principal.Id = PrincipalGroup.PrincipalId
                      WHERE Principal.username = @idutente
                      AND Menu.ParentId = @idpadre
                      AND GroupMenu.ToExec = 1
                      ORDER BY Menu.Ordering";

                paras.Add(new SqlParameter("idutente", userName));
                paras.Add(new SqlParameter("idpadre", idPadre));
                command.CommandText = s;
                command.Parameters.AddRange(paras.ToArray());

                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int i = 0;
                        Menu menu = new Menu();
                        menu.Id = reader.GetInt64(i++);
                        menu.Description = reader.GetNullableString(i++);
                        menu.Icon = reader.GetNullableString(i++);
                        menu.Name = reader.GetNullableString(i++);
                        try
                        {
                            if (!keys.Contains(menu.Id))
                            {
                                keys.Add(menu.Id);
                                result.Add(menu);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Mangio Eccezione
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }
    }
}
