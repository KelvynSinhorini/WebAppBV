namespace WebAppBV.Helpers
{
    public class OperationResult
    {
        public OperationResult(string message, bool sucess = true)
        {
            Message = message;
            Sucess = sucess;
        }

        public string Message { get; set; }
        public bool Sucess { get; set; }
    }
}
