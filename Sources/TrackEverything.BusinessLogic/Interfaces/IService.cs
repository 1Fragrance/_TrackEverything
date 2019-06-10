using System.Collections.Generic;

namespace TrackEverything.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface that contains all basic methods for business logic
    /// </summary>
    public interface IService<T> where T : class
    {
        /// <summary>
        /// Method that return  instance of entity by id
        /// </summary>
        T Get(int id);

        /// <summary>
        /// Method that return all instances of entity
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Method that add instance of entity to database
        /// </summary>
        int Add(T item);


        /// <summary>
        /// Method that delete instance of entity from database
        /// </summary>
        void Delete(int id);


        /// <summary>
        /// Method that update instance of entity in database
        /// </summary>
        int Update(T project, int id);
    }
}