using System;

namespace Business
{
    public readonly struct Result<TValue, TError>
    {
        private readonly TValue? value;
        private readonly TError? error;

        public Result(TValue value)
        {
            IsError = false;
            value = value;
            error = default;
        }
        public Result(TError error)
        {
            IsError = true;
            value = default;
            error = error;
        }

        public bool IsError { get; set; }

        public bool IsSuccess => !IsError;

        public static implicit operator Result<TValue,TError>(TValue value) => new (value);
        public static implicit operator Result<TValue, TError>(TError error) => new(error);

        public TResult Match<TResult>(
        
                Func<TValue, TResult> success, Func<TError, TResult> failure) =>
                !IsError ? success(value) : failure(error);
        }
    }
}

