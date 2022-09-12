using System.Collections;

namespace ChanisterWpf
{
    public class ExtendedStack : Stack
    {
        /// <summary>
        ///     Removes all of the speciefied object references.
        /// </summary>
        /// <param name="obj">Object reference to remove.</param>
        /// <returns>A new stack without the referenced object.</returns>
        public ExtendedStack Remove(object obj)
        {
            if (!Contains(obj)) return this;
            ExtendedStack extendedStack = new();
            foreach (object o in this)
            {
                if (o != obj) extendedStack.Push(o);
            }
            return extendedStack;
        }
    }
}
