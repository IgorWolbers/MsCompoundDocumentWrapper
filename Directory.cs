using System;
using System.IO;
using System.Collections.Generic;

namespace Storage.CompoundDocument
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Directory
    {
        /// <summary>
        /// Gets or sets the directory id.
        /// </summary>
        /// <value>The directory id.</value>
        public int DirectoryId { get; private set; }
        /// <summary>
        /// Gets or sets the name of the directory.
        /// </summary>
        /// <value>The name of the directory.</value>
        public string DirectoryName { get; private set; }
        /// <summary>
        /// Gets or sets the size of name char buffer.
        /// </summary>
        /// <value>The size of name char buffer.</value>
        public int SizeOfNameCharBuffer { get; private set; }
        /// <summary>
        /// Gets or sets the type of the entry.
        /// </summary>
        /// <value>The type of the entry.</value>
        public TypeOfEntry EntryType { get; private set; }
        /// <summary>
        /// Gets or sets the color of the node.
        /// </summary>
        /// <value>The color of the node.</value>
        public NodeColour NodeColor { get; private set; }
        /// <summary>
        /// Gets or sets the dir id left inside child node.
        /// </summary>
        /// <value>The dir id left inside child node.</value>
        public int DirIdLeftInsideChildNode { get; private set; }
        /// <summary>
        /// Gets or sets the dir id right inside child node.
        /// </summary>
        /// <value>The dir id right inside child node.</value>
        public int DirIdRightInsideChildNode { get; private set; }
        /// <summary>
        /// Gets or sets the dir id of root node entry.
        /// </summary>
        /// <value>The dir id of root node entry.</value>
        public int DirIdOfRootNodeEntry { get; private set; }
        /// <summary>
        /// Gets or sets the unique id of storage.
        /// </summary>
        /// <value>The unique id of storage.</value>
        public int UniqueIdOfStorage { get; private set; }
        /// <summary>
        /// Gets or sets the user flags.
        /// </summary>
        /// <value>The user flags.</value>
        public byte[] UserFlags { get; private set; }
        /// <summary>
        /// Gets or sets the time stamp of creation.
        /// </summary>
        /// <value>The time stamp of creation.</value>
        public byte[] TimeStampOfCreation { get; private set; }
        /// <summary>
        /// Gets or sets the time stamp of last modified.
        /// </summary>
        /// <value>The time stamp of last modified.</value>
        public byte[] TimeStampOfLastModified { get; private set; }
        /// <summary>
        /// Gets or sets the sector id of first sector.
        /// </summary>
        /// <value>The sector id of first sector.</value>
        public int SectorIdOfFirstSector { get; private set; }
        /// <summary>
        /// Gets or sets the total stream size in bytes.
        /// </summary>
        /// <value>The total stream size in bytes.</value>
        public int TotalStreamSizeInBytes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value><c>true</c> if this instance is root; otherwise, <c>false</c>.</value>
        public bool IsRoot
        {
            get
            {
                return DirectoryName.Equals("Root Entry", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is stored in short stream.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is stored in short stream; otherwise, <c>false</c>.
        /// </value>
        public bool IsStoredInShortStream
        {
            get
            {
                if (IsRoot)
                    return false;
                return TotalStreamSizeInBytes < 4096;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Directory"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="inStream">The in stream.</param>
        public Directory(int id, Stream inStream)
        {
            DirectoryId = id;
            List<byte> byteList = new List<byte>();
            for (int i = 0; i < 64; i++)
            {
                var byteRead = (byte)inStream.ReadByte(); ;
                if (byteRead == (byte)0x00)
                    continue;
                byteList.Add(byteRead);
            }
            if (byteList.Count > 0 &&
                (byteList[0] == 1 || byteList[0] == 2))
                byteList.RemoveAt(0);
            var bytes = byteList.ToArray();

            DirectoryName = System.Text.Encoding.UTF8.GetString(bytes).Trim();
            
            bytes = inStream.ReadStreamPart(2);
            SizeOfNameCharBuffer = bytes.ConvertToInt();
            EntryType = (TypeOfEntry) inStream.ReadStreamPart(1)[0];
            NodeColor = (NodeColour)inStream.ReadStreamPart(1)[0];
            DirIdLeftInsideChildNode = inStream.ReadStreamPart(4).ConvertToInt();
            DirIdRightInsideChildNode = inStream.ReadStreamPart(4).ConvertToInt();
            DirIdOfRootNodeEntry = inStream.ReadStreamPart(4).ConvertToInt();
            UniqueIdOfStorage = inStream.ReadStreamPart(16).ConvertToInt();
            UserFlags = inStream.ReadStreamPart(4);
            TimeStampOfCreation = inStream.ReadStreamPart(8);
            TimeStampOfLastModified = inStream.ReadStreamPart(8);
            SectorIdOfFirstSector = inStream.ReadStreamPart(4).ConvertToInt();
            TotalStreamSizeInBytes = inStream.ReadStreamPart(4).ConvertToInt();
            inStream.ReadStreamPart(4).ConvertToInt();
        }
        public override string ToString()
        {
            return string.Format("{0} - {1:g} - {2}", DirectoryId, EntryType, DirectoryName);
        }
    }
}
