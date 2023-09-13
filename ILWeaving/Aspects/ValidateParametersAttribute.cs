using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AOPDemo.ILWeaving.Aspects
{
    /// <summary>
    /// Provides an aspect used to signal that the decorated method supports attribute-specified
    /// validation on one or more of its parameters. This class cannot be inherited.
    /// </summary>
    [PSerializable]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class)]
    [LinesOfCodeAvoided(15)]
    public sealed class ValidateParametersAttribute : OnMethodBoundaryAspect
    {
        private List<ParameterInfo> _parameterMap = new();
        private Dictionary<int, List<ParameterValidationAttribute>> _validationMap = new();

        /// <summary>
        /// Method invoked at build-time to build the parameter mapping information stored for later
        /// use by this aspect.
        /// </summary>
        /// <param name="method">Method to which this aspect is applied.</param>
        /// <param name="aspectInfo">Reserved parameter.</param>
        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            base.CompileTimeInitialize(method, aspectInfo);

            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                _parameterMap.Add(parameters[i]);

                IEnumerable<ParameterValidationAttribute> validationAttributes = parameters[i].GetCustomAttributes(false).OfType<ParameterValidationAttribute>();

                foreach (ParameterValidationAttribute validationAttribute in validationAttributes)
                {
                    MapAttribute(i, validationAttribute);
                }
            }
        }

        /// <summary>
        /// Method executed prior to the applied method's execution.
        /// </summary>
        /// <param name="args">
        /// Arguments specifying which method is being executed, information regarding the method's
        /// arguments, and the flow of execution.
        /// </param>
        public override void OnEntry(MethodExecutionArgs args)
        {
            if (args == null)
            {
                return;
            }

            try
            {
                for (int i = 0; i < args.Arguments.Count; i++)
                {
                    if (!_validationMap.ContainsKey(i)) { continue; }

                    foreach (ParameterValidationAttribute validationAttribute in _validationMap[i])
                    {
                        validationAttribute.ValidateParameter(args.Method.Name, _parameterMap[i].Name, args.Arguments[i]);
                    }
                }

                base.OnEntry(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Maps the provided <see cref="ParameterValidationAttribute"/> object to the specified key.
        /// </summary>
        /// <param name="key">The key to associate with the <see cref="ParameterValidationAttribute"/> object.</param>
        /// <param name="validationAttribute">The validation attribute to map.</param>
        private void MapAttribute(int key, ParameterValidationAttribute validationAttribute)
        {
            if (!_validationMap.ContainsKey(key))
            {
                _validationMap.Add(key, new List<ParameterValidationAttribute>());
            }

            List<ParameterValidationAttribute> attributeList = _validationMap[key];
            attributeList.Add(validationAttribute);
        }
    }

    /// <summary>
    /// Provides a custom attribute that performs a type of validation on
    /// the decorated parameter on the behalf of an aspect. This class is abstract.
    /// </summary>
    [PSerializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ParameterValidationAttribute : Attribute
    {
        /// <summary>
        /// Performs validation on the object provided, throwing an exception if validation fails.
        /// </summary>
        /// <param name="methodName">The name of the method the parameter belongs to.</param>
        /// <param name="parameterName">The <see cref="ParameterInfo"/> name of the decorated parameter.</param>
        /// <param name="value">An instance of the decorated parameter.</param>
        public abstract void ValidateParameter(string methodName, string parameterName, object value);
    }

    [PSerializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullAttribute : ParameterValidationAttribute
    {
        /// <inheritdoc/>
        public override void ValidateParameter(string methodName, string parameterName, object value)
        {
            if (String.IsNullOrEmpty(parameterName))
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }

    [PSerializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullOrEmptyAttribute : ParameterValidationAttribute
    {
        /// <inheritdoc/>
        public override void ValidateParameter(string methodName, string parameterName, object value)
        {
            if (String.IsNullOrEmpty(parameterName))
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value is string strValue && string.IsNullOrEmpty(strValue))
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }

    [PSerializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NumericOnlyStringAttribute : ParameterValidationAttribute
    {
        /// <inheritdoc/>
        public override void ValidateParameter(string methodName, string parameterName, object value)
        {
            if (String.IsNullOrEmpty(parameterName))
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            string strValue = value as string;
            if (strValue != null && string.IsNullOrEmpty(strValue))
            {
                throw new ArgumentNullException(parameterName);
            }

            bool isNum = Double.TryParse(strValue, out double retNum);

            if (!isNum)
            {
                throw new ArgumentException($"Input is required to be numeric. Value: {strValue}", parameterName);
            }
        }
    }

    [PSerializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ValidDateTimeStringAttribute : ParameterValidationAttribute
    {
        /// <inheritdoc/>
        public override void ValidateParameter(string methodName, string parameterName, object value)
        {
            if (String.IsNullOrEmpty(parameterName))
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            string strValue = value as string;
            if (strValue != null && string.IsNullOrEmpty(strValue))
            {
                throw new ArgumentNullException(parameterName);
            }

            bool isValidDateTime = DateTime.TryParse(strValue, out DateTime dt);

            if (!isValidDateTime)
            {
                throw new ArgumentException($"Invalid DateTime format. Value: {strValue}", parameterName);
            }
        }
    }

    [PSerializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ValidZipCodeAttribute : ParameterValidationAttribute
    {
        /// <inheritdoc/>
        public override void ValidateParameter(string methodName, string parameterName, object value)
        {
            if (String.IsNullOrEmpty(parameterName))
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            string strValue = value as string;

            if (strValue != null && string.IsNullOrEmpty(strValue))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (strValue.Length < 5)
            {
                throw new ArgumentException($"Invalid length for known Zip Codes. Value: {strValue}", parameterName);
            }

            strValue = strValue.ToUpper();

            //This pattern matches both US and Canadian formats for Postal/Zip Codes that we currently have in our ZipCodeDB.
            string pattern = @"(^\d{5}(\-?\d{4})?$)|([ABCEGHJ-NPRSTVXY]\d[A-Z])+["" ""]{1}(\d[A-Z]\d)";

            bool isMatch = Regex.IsMatch(strValue, pattern);

            if (!isMatch)
            {
                throw new ArgumentException($"Invalid format for known Zip Codes. Value: {strValue}", parameterName);
            }
        }
    }
}
