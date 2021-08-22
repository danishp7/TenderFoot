using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Enums
{
    public enum ApplicationStatus
    {
        Post,
        Apply,
        InitialScreening,
        Accept,
        InProcess,
        Finalizing,
        Complete
    }
    public enum Issuer
    {
        AppIssuer
    }
    public enum JobType
    {
        FrontEnd,
        BackEnd,
        FullStack,
        Database
    }
    public enum Salary : ulong
    {
        Junior = 4,
        Associate = 8
    }
    public enum MartialStatus
    {
        Single,
        Married
    }
    public enum Requirement
    {
        HandlingDB,
        HandlingJs,
        HandlingDocker
    }
    public enum Role
    {
        Admin,
        AppUser
    }
    public enum Employer
    {
        Nakisa,
        Xavor
    }
}
