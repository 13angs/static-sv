namespace static_sv.Interfaces
{
    public interface IRequestValidator {
        public bool Validate(object content, string signature);
    }
}