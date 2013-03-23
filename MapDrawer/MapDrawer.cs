using System;
using System.Collections.Generic;
using System.Text;

namespace MapDrawer
{
    public class MapDrawer<TElement,TLateral>
        where TElement:IEquatable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapDrawer&lt;TElement, TLateral&gt;"/> class.
        /// </summary>
        public MapDrawer()
        {
        }
        public TElement StartElement;
    }
    public delegate void ElementAttributesGetHandler<TElement>(TElement element);
}
