namespace Storage.CompoundDocument
{
    /// <summary>
    /// 
    /// </summary>
    public enum ByteOrderIdentifiers
    {
        /// <summary>
        /// 
        /// </summary>
        LittleEndian,
        /// <summary>
        /// 
        /// </summary>
        BigEndian
    }
    /// <summary>
    /// 
    /// </summary>
    public enum NodeColour
    {
        /// <summary>
        /// 
        /// </summary>
        Red = 0,
        /// <summary>
        /// 
        /// </summary>
        Black = 1
    }
    /// <summary>
    /// 
    /// </summary>
    public enum TypeOfEntry
    {
        /// <summary>
        /// 
        /// </summary>
        Empty = 0,
        /// <summary>
        /// 
        /// </summary>
        UserStorage = 1,
        /// <summary>
        /// 
        /// </summary>
        UserStream = 2,
        /// <summary>
        /// 
        /// </summary>
        LockBytes = 3,
        /// <summary>
        /// 
        /// </summary>
        Property = 4,
        /// <summary>
        /// 
        /// </summary>
        RootStorage = 5
    }
    /// <summary>
    /// 
    /// </summary>
    public enum SpecialSecIds : int
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// Free SecID Free sector, may exist in the file, but is not part of any stream
        /// </summary>
        Free = -1,
        /// <summary>
        /// Trailing SecID in a SecID chain (➜3.2)
        /// </summary>
        EndOfChain = -2,
        /// <summary>
        /// Sector is used by the sector allocation table (➜5.2)
        /// </summary>
        SAT = -3,
        /// <summary>
        ///  Sector is used by the master sector allocation table (➜5.1) 
        /// </summary>
        MSAT = -4
    }
}
