using System.ComponentModel;
using Framework.Data;
using UnityEngine.Events;

namespace Framework.References
{
    public abstract class DataReference<T>
    {
        public bool useConstantValue;
        public T constantValue;

        public abstract Variable<T> Variable { get; }

        public T Value
        {
            get { return useConstantValue ? constantValue : Variable.Value; }

            set
            {
                if (Variable != null) {
                    Variable.Value = value;
                } else
                {
                    var oldValue = constantValue;
                    constantValue = value;
                    if (!oldValue.Equals(constantValue))
                    {
                        constantValueChanged?.Invoke(oldValue, constantValue);
                    }
                }
            }
        }

        public event UnityAction<T, T> valueChanged
        {
            add
            {
                if (Variable != null) {
                    Variable.valueChanged += value;
                }

                constantValueChanged += value;
            }
            remove
            {
                if (Variable != null) {
                    Variable.valueChanged -= value;
                }

                constantValueChanged -= value;
            }
        }

        private event UnityAction<T, T> constantValueChanged; 

        public static implicit operator T (DataReference<T> reference)
        {
            return reference.Value;
        }
    }
}