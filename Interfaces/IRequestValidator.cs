using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IRequestValidator {
        public string Validate(object content, string signature);
    }
}