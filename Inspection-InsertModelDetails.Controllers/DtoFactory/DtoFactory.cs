using System;
using InspectionInsertModelDetails.Messages;
using InspectionInsertModelDetails.Messages.Dtos;

namespace InspectionInsertModelDetails.Controllers.DtoFactory;
public class DtoFactory : IDtoFactory
{
    public object CreateDto(string dtoType, params object[] args)
    {
        if (args == null || args.Length == 0)
            throw new ArgumentException("Arguments cannot be null or empty.");

        switch (dtoType.ToLower())
        {
        
            case "insertmodeldetailsdto":
                if (args.Length < 2 || !(args[0] is string))
                    throw new ArgumentException("Invalid arguments for messageRequest.");

                return new ModelDetailsRequest
                {
                    Model_URL = (string)args[0],
                    InspectionName = (string)args[1],
                    status = (string)args[2]
                };

            default:
                throw new ArgumentException($"Invalid DTO type: {dtoType}");
        }
    }

    public object UseDto(string dtoType, object dto)
    {
        if (dto == null)
            throw new ArgumentException("DTO object cannot be null.");

        switch (dtoType.ToLower())
        {
            case "insertmodeldetailsdto":
                return dto;
            default:
                throw new ArgumentException($"Invalid DTO type: {dtoType}");
        }
    }
}
