namespace DomainCore.Helper.Constant
{
    public class UrlConstant
    {
        #region Web Api Url

        public const string LoginUrl = "/Security.Api/AuthenticateUser";

        #endregion

        #region Controller Route

        public const string Authorize = "Authorize";
     


        public const string BalanceView = "Balance";
        public const string Deposit = "Deposit";
        public const string Withdraw = "Withdraw";

        #endregion

        #region View Constant

       
        public const string TransactionViewCshtml = "~/Views/Transaction/Transaction.cshtml";
        public const string AuthenticationViewCshtml = "~/Views/Authentication/Index.cshtml";
        public const string BalanceViewCshtml = "~/Views/Balance/Balance.cshtml";
        public const string SimulatorViewCshtml = "~/Views/simulator/simulator.cshtml";
        public const string PuttyViewCshtml = "~/Views/putty/putty.cshtml";
       
        #endregion
    }
}