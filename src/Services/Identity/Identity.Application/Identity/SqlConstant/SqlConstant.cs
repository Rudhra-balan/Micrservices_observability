

namespace Identity.Application.Identity.SqlConstant
{
    public static class SqlConstant
    {
        public const string Authentication = "select UserID, UserName,FirstName,LastName,UserEmail,AccountNumber from userinfo where UserName = @iUsername and Password = @iPassword;";
        public const string GetUserInfoById = "select UserID, UserName,FirstName,LastName,UserEmail,AccountNumber from userinfo where UserID = @iUserID ;";
       
        public const string SQL_REFRESH_TOKEN_EXPIRY_TIME = "SELECT  DATEDIFF(MINUTE, token_timestamp, current_timestamp) from auth_tokens where refresh_token = @iRefreshToken;;";

        public const string RegisterUser = @"INSERT INTO userinfo (UserName,FirstName,LastName,UserEmail,Password,accountNumber)
                                                SELECT @iUserName,@iFirstName,@iLastName,@iUserEmail,@iPassword, LEFT(CAST(RAND()*1000000000 AS INT),7)
                                                Where not exists(select UserName from userinfo where UserName=@iUserName);";

        public const string InsertRefreshToken = "insert into auth_tokens (refresh_token,access_token,userId) values (@iRefreshToken,@iAccessToken,@iUserId)";
        public static readonly string SQL_UPDATE_AUTHENTICATION_TOKEN =
           @"Update auth_tokens set access_token = @iAccessToken, refresh_token = @iNewRefreshToken,token_timestamp = CURRENT_TIMESTAMP where refresh_token= @iOldRefreshToken;";
    }
}
