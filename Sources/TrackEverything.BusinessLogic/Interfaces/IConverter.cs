namespace TrackEverything.BusinessLogic.Interfaces
{
    /// <summary>
    /// Generic interface that contains requirements of converters 
    /// </summary>
    public interface IConverter<TIn, TOut>
        where TIn : class
        where TOut : class
    {

        /// <summary>
        /// Method that convert one type of entity to another
        /// </summary>
        TOut Convert(TIn @in);
    }
}