using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public class MxService
{
    public void Validate(Mx009 mx009)
    {
        var validationContext = new ValidationContext(mx009);

        Validator.ValidateObject(mx009, validationContext, true);
    }
}