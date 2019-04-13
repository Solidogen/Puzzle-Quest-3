using System;

namespace Puzzle_Quest_3.Assets.Scripts.Extensions
{
    public static class ObjectExtensions
    {
        public static T Also<T>(this T self, Action<T> block)
        {
            block(self);
            return self;
        }
    }
}