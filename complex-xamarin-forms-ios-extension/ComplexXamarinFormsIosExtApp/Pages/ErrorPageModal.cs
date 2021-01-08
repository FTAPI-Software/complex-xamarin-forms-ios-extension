namespace ComplexXamarinFormsIosExtApp.Pages
{
    public class ErrorPageModal: ErrorPage
    {
        #region Delegates

        public delegate void ClosedHandler();

        #endregion

        #region Events

        public event ClosedHandler Closed;

        #endregion

        #region Constructors

        public ErrorPageModal(string errorMessage) : base(errorMessage)
        {

        }

        #endregion

        #region Methods

        protected override void Close()
        {
            this.Closed?.Invoke();
        }

        #endregion
    }
}
