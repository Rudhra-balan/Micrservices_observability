using DomainCore.Enumeration.ErrorEn;

namespace DomainCore.Models.Response
{
    public class WebClientResponse
    {
        #region Public Members

        public int ErrorId { get; set; }

        public string ErrorDescription { get; set; }

        public bool IsOperationSuccess { get; set; }

        public ErrorDisplayTypeEnum DisplayType { get; set; }

        public object SourceObject { get; set; }

        public string RedirectUrl { get; set; }

        #endregion
    }
}