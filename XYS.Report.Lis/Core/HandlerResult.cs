﻿namespace XYS.Report.Lis.Core
{
    public enum HandlerResult : int
    {
        Fail = 0,
        HasErrorButContinue = 1,
        Continue = 2,
        Success = 3
    }
}