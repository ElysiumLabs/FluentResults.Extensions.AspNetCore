using FluentResults;
using System;
using System.Collections.Generic;

namespace FluentProblemDetails
{
    public class ResultErrorProblemDetailsOptions
    {
        /// <summary>
        /// To use or not the default Result<T> action filter, that enables automatic ProblemDetails generation
        /// You can use your own filter if u with and create problemdetails using factory
        /// </summary>
        public bool UseDefaultActionModelConventions { get; set; } = true;

        /// <summary>
        /// Creates recursivily problemdetails inside "reasons" property
        /// </summary>
        public bool Recursive { get; set; } = true;

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
        public Func<IError, Type, string> GetTypeMap { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, string> GetTitleMap { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, string> GetDetailMap { get; set; }

        /// <summary>
        /// Self explonatory
        /// </summary>
        public Func<IError, Type, Dictionary<string, object>> GetExtensionsMap { get; set; }



    }
}
