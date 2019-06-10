using System.Collections.Generic;

namespace TrackEverything.Storage.Core.Interfaces
{
    /// <summary>
    /// Interface that contains all basic methods for repositories
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Method that return all instances of entity
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Method that return instance of entity by id
        /// </summary>
        T Get(int id);

        /// <summary>
        /// Method that add instance of entity to the database
        /// </summary>
        int Create(T item);

        /// <summary>
        /// Method that update instance of entity in the database
        /// </summary>
        int Update(T item, int id);

        /// <summary>
        /// Method that delete instance of entity from the database
        /// </summary>
        void Delete(int id);
    }
}