/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.SpellCheck
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// The Exception will be shown to client.
    /// </summary>
    public class ClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        public ClientException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        public ClientException(string message)
            : base(message)
        {
            this.Error = new ErrorResponse()
            {
                Errors = new ClientError[1]
                { 
                    new ClientError()
                    {
                        Code = HttpStatusCode.InternalServerError.ToString(),
                        Message = message
                    } 
                }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="httpStatus">The Http Status code.</param>
        public ClientException(string message, HttpStatusCode httpStatus)
            : base(message)
        {
            this.HttpStatus = httpStatus;

            this.Error = new ErrorResponse()
            {
                Errors = new ClientError[1]
                { 
                    new ClientError()
                    {
                        Code = httpStatus.ToString(),
                        Message = message
                    } 
                }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Error = new ErrorResponse()
            {
                Errors = new ClientError[1]
                { 
                    new ClientError()
                    {
                        Code = HttpStatusCode.InternalServerError.ToString(),
                        Message = message
                    } 
                }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="httpStatus">The http status.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClientException(string message, string errorCode, HttpStatusCode httpStatus, Exception innerException)
            : base(message, innerException)
        {
            this.HttpStatus = httpStatus;

            this.Error = new ErrorResponse()
            {
                Errors = new ClientError[1]
                { 
                    new ClientError()
                    {
                        Code = errorCode,
                        Message = message
                    } 
                }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="error">The error entity.</param>
        /// <param name="httpStatus">The http status.</param>
        public ClientException(ErrorResponse error, HttpStatusCode httpStatus)
        {
            this.Error = error;
            this.HttpStatus = httpStatus;
        }

        /// <summary>
        /// Gets http status of http response.
        /// </summary>
        public HttpStatusCode HttpStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the httpError message.
        /// </summary>
        public ErrorResponse Error { get; set; }

        /// <summary>
        /// Create Client Exception of Bad Request.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <returns>Client Exception Instance.</returns>
        public static ClientException BadRequest(string message)
        {
            return new ClientException(
                         new ErrorResponse()
                         {
                             Errors = new ClientError[1]
                             { 
                                 new ClientError()
                                 {
                                     Code = HttpStatusCode.BadRequest.ToString(),
                                     Message = message
                                 } 
                             }
                         },
                         HttpStatusCode.BadRequest);
        }
    }
}
