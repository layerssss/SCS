using System;
using System.Collections.Generic;
using System.Text;

namespace SCS
{

    /// <summary>
    /// A regular exp or a part of it.
    /// </summary>
    class regularPart
    {
        public regularPart[] SubParts;
        /// <summary>
        /// Indicating the part is or not optional.
        /// Default:false
        /// </summary>
        public bool Optional;
        public bool Repeat;
        /// <summary>
        /// Initializes a new instance of the <see cref="regularPart"/> class.
        /// </summary>
        regularPart()
        {
            this.Optional = false;
            this.Repeat = false;
        }
        public regularPart(char c)
            : this()
        {
            this.Char = c;
        }
        public regularPart(regularPartType type, params regularPart[] subParts) :
            this()
        {
            this.Type = type;
            this.SubParts = subParts;
        }
        public regularPart(regularPartType type, bool optional, params regularPart[] subParts)
            : this(type, subParts)
        {
            this.Optional = optional;
        }
        public regularPart(regularPartType type, bool optional, bool repeat, params regularPart[] subParts)
            : this(type, optional, subParts)
        {
            this.Repeat = repeat;
        }
        public char Char;
        /// <summary>
        /// default:SpecifyChar
        /// </summary>
        public regularPartType Type;
    }
    enum regularPartType
    {
        SpecifyChar,
        Concatenation,
        MultiChoice
    }
}
