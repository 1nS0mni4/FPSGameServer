using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB {
    public class DbCommands {

        public static void InitializeDB(bool forceReset = false) {
            using(AppDbContext db = new AppDbContext()) {
                if(!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                Console.WriteLine($"DB Initialized!");
            }
        }

        #region Account Database Commands

        public static pAuthResult AccountValidationCheck(C_Login_Access access) {
            Console.WriteLine($"AccountValidationCheck Started!");
            pAuthResult result = new pAuthResult();

            using(AppDbContext db = new AppDbContext()) {
                string userID = access.Id;
                string userPW = access.Pw;

                UserAccount account = db.UserAccounts.SingleOrDefault(data => data.ID == userID);

                if(account == null) {                           //계정이 존재하지 않음
                    result.ErrorCode = NetworkError.Noaccount;
                    result.AuthCode = -1;
                    return result;
                }

                if(account.Password.Equals(userPW) == false) {  //비밀번호 오류
                    result.ErrorCode = NetworkError.InvalidPassword;
                    result.AuthCode = -1;
                    return result;
                }

                if(account.AuthCode.Equals(-1) == false) {
                    result.ErrorCode = NetworkError.Overlap;
                    result.AuthCode = -1;
                    return result;
                }

                account.AuthCode = Random.Shared.Next(0, 65535);
                db.SaveChanges();

                result.ErrorCode = NetworkError.Success;
                result.AuthCode = account.AuthCode;
                return result;
            }
        }

        public static bool AccountCreate(C_Login_Register register) {
            Console.WriteLine($"AccountCreate Started!");
            using(AppDbContext db = new AppDbContext()) {
                UserAccount regist = db.UserAccounts.SingleOrDefault(account => account.ID == register.Id);

                if(regist != null)
                    return false;

                regist = new UserAccount() {
                    ID = register.Id,
                    Password = register.Pw
                };

                db.UserAccounts.Add(regist);
                db.SaveChanges();

                return true;
            }
        }

        public static bool Disconnect(int authCode) {
            using(AppDbContext db = new AppDbContext()) {
                //UserAuth auth = db.UserAuths.SingleOrDefault(i => i.AuthCode == authCode);
                UserAccount account = db.UserAccounts.SingleOrDefault(i => i.AuthCode == authCode);

                if(account == null)
                    return false;

                account.AuthCode = -1;
                db.SaveChanges();

                return true;
            }
        }

        #endregion
    }
}
