namespace BankingSystem.Application.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get;  }

        public bool IsFailure => !IsSuccess;        
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {

            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Success result cannot have an error");

            if (string.IsNullOrEmpty(error) && !isSuccess)
                throw new InvalidOperationException("Failure result must have an error");


            this.IsSuccess = isSuccess;
            this.Error = error;
        }


    }
}
