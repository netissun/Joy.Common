using NVelocity;
using NVelocity.App;
using NVelocity.Exception;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joy.Common.Utilities
{
    public class NVelocityMemoryEngine : VelocityEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NVelocityMemoryEngine"/> class.
        /// </summary>
        /// <param name="cacheTamplate">if set to <c>true</c> [cache tamplate].</param>
        /// 
        public NVelocityMemoryEngine()
        {
            this.Init();
        }

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public string Process(IDictionary context, string template)
        {
            using (StringWriter writer = new StringWriter())
            {
                try
                {
                    this.Evaluate(new VelocityContext(new Hashtable(context)), writer, "mystring", template);
                }
                catch (ParseErrorException pe)
                {
                    return pe.Message;
                }
                catch (MethodInvocationException mi)
                {
                    return mi.Message;
                }

                return writer.ToString();
            }
        }

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="template">The template.</param>
        public void Process(IDictionary context, TextWriter writer, string template)
        {
            try
            {
                this.Evaluate(new VelocityContext(new Hashtable(context)), writer, "mystring", template);
            }
            catch (ParseErrorException pe)
            {
                writer.Write(pe.Message);
            }
            catch (MethodInvocationException mi)
            {
                writer.Write(mi.Message);
            }
        }
    }
}
