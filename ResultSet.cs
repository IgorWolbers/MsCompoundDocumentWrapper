namespace Storage.CompoundDocument
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ResultSet
    {
        /// <summary>
        /// Gets or sets the out directory.
        /// </summary>
        /// <value>The out directory.</value>
        public Directory OutDirectory { get; private set; }
        /// <summary>
        /// Gets or sets the out stream.
        /// </summary>
        /// <value>The out stream.</value>
        public System.IO.Stream OutStream { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSet"/> class.
        /// </summary>
        /// <param name="outDirectory">The out directory.</param>
        /// <param name="outStream">The out stream.</param>
        public ResultSet(Directory outDirectory, System.IO.Stream outStream)
        {
            OutDirectory = outDirectory;
            OutStream = outStream;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        public bool HasContent
        {
            get
            {
                return OutStream != null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value><c>true</c> if this instance is root; otherwise, <c>false</c>.</value>
        public bool IsRoot
        {
            get
            {
                return OutDirectory.IsRoot;
            }
        }
        /// <summary>
        /// Closes the stream.
        /// </summary>
        public void CloseStream()
        {
            if (HasContent)
            {
                OutStream.Close();
                OutStream.Dispose();
                OutStream = null;
            }
        }
    }
}
