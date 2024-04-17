using System;
using System.Collections.Generic;

namespace FluentResults.Extensions.AspNetCore.Options
{
    public class ResultExtensionsOptions
    {
        /// <summary>
        /// To use or not the default Result<T> action filter, that enables automatic ProblemDetails generation
        /// You can use your own filter if u with and create problemdetails using factory
        /// </summary>
        public bool UseDefaultActionModelConventions { get; set; } = true;

        /// <summary>
        /// Creates recursivily problemdetails inside "reasons" property
        /// </summary>
        public bool ErrorRecursive { get; set; } = true;

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Dictionary<Type, int> ErrorTypeToStatusCodeMap { get; set; } = new Dictionary<Type, int>();

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<Type, int> ErrorTypeToStatusCodeMapFallback { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, string> ErrorGetTypeMap { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, string> ErrorGetTitleMap { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, string> ErrorGetDetailMap { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, Dictionary<string, object>> ErrorGetExtensionsMap { get; set; }


        public Func<IResultBase, object, object> SuccessValueMap { get; set; }
    }
}
