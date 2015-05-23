using System;
using System.IO;

namespace Storage.CompoundDocument
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentHeader
    {
        /// <summary>
        /// Gets or sets the revision number.
        /// </summary>
        /// <value>The revision number.</value>
        public int RevisionNumber { get; private set; } // 2
        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        /// <value>The version number.</value>
        public int VersionNumber { get; private set; }
        /// <summary>
        /// Gets or sets the byte order identifier.
        /// </summary>
        /// <value>The byte order identifier.</value>
        public ByteOrderIdentifiers ByteOrderIdentifier { get; private set; } // 4
        /// <summary>
        /// Gets or sets the size of sector.
        /// </summary>
        /// <value>The size of sector.</value>
        public int SizeOfSector { get; private set; }
        /// <summary>
        /// Gets or sets the size of sector in short stream.
        /// </summary>
        /// <value>The size of sector in short stream.</value>
        public int SizeOfSectorInShortStream { get; private set; } // 6
        /// <summary>
        /// Gets or sets the total number of sectors used for sector allocation table.
        /// </summary>
        /// <value>The total number of sectors used for sector allocation table.</value>
        public int TotalNumberOfSectorsUsedForSectorAllocationTable { get; private set; } // 8
        /// <summary>
        /// Gets or sets the special value of first sector.
        /// </summary>
        /// <value>The special value of first sector.</value>
        public SpecialSecIds SpecialValueOfFirstSector { get; private set; }
        /// <summary>
        /// Gets or sets the sec ID of first sector of directory.
        /// </summary>
        /// <value>The sec ID of first sector of directory.</value>
        public int SecIDOfFirstSectorOfDirectory { get; private set; }
        /// <summary>
        /// Gets or sets the minimum size of standard stream.
        /// </summary>
        /// <value>The minimum size of standard stream.</value>
        public int MinimumSizeOfStandardStream { get; private set; }
        /// <summary>
        /// Gets or sets the special value of first sector of the short sector.
        /// </summary>
        /// <value>The special value of first sector of the short sector.</value>
        public SpecialSecIds SpecialValueOfFirstSectorOfTheShortSector { get; private set; } // 12
        /// <summary>
        /// Gets or sets the sec ID of first sector of the short sector.
        /// </summary>
        /// <value>The sec ID of first sector of the short sector.</value>
        public int SecIDOfFirstSectorOfTheShortSector { get; private set; } // 12
        /// <summary>
        /// Gets or sets the total number of sectors used for sector allocation table of the short sector.
        /// </summary>
        /// <value>
        /// The total number of sectors used for sector allocation table of the short sector.
        /// </value>
        public int TotalNumberOfSectorsUsedForSectorAllocationTableOfTheShortSector { get; private set; }
        /// <summary>
        /// Gets or sets the special value of first sector of the master sector allocation table.
        /// </summary>
        /// <value>
        /// The special value of first sector of the master sector allocation table.
        /// </value>
        public SpecialSecIds SpecialValueOfFirstSectorOfTheMasterSectorAllocationTable { get; private set; } // 14
        /// <summary>
        /// Gets or sets the sec ID of first sector of the master sector allocation table.
        /// </summary>
        /// <value>
        /// The sec ID of first sector of the master sector allocation table.
        /// </value>
        public int SecIDOfFirstSectorOfTheMasterSectorAllocationTable { get; private set; } // 14
        /// <summary>
        /// Gets or sets the total number of sectors used for sector allocation table for the master sector allocation table.
        /// </summary>
        /// <value>
        /// The total number of sectors used for sector allocation table for the master sector allocation table.
        /// </value>
        public int TotalNumberOfSectorsUsedForSectorAllocationTableForTheMasterSectorAllocationTable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentHeader"/> class.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        public DocumentHeader(Stream inStream)
        {
            byte[] tempBytes;
            int tempInt;

            var compoundDocumentFileIdentifier = inStream.ReadStreamPart(8); // 0
            if (SetIsCompoundDocument(compoundDocumentFileIdentifier) == false)
                return;
            inStream.ReadStreamPart(16); // unique identifier, not used

            RevisionNumber = inStream.ReadStreamPart(2).ConvertToInt();

            VersionNumber = inStream.ReadStreamPart(2).ConvertToInt();
            
            tempBytes = inStream.ReadStreamPart(2); // 4
            if (tempBytes[0] == 0xFE && tempBytes[1] == 0xFF)
                ByteOrderIdentifier = ByteOrderIdentifiers.LittleEndian;
            else if (tempBytes[0] == 0xFF && tempBytes[1] == 0xFE)
                ByteOrderIdentifier = ByteOrderIdentifiers.BigEndian;

            SizeOfSector = (int)Math.Pow(2, inStream.ReadStreamPart(2).ConvertToInt());
            
            SizeOfSectorInShortStream = (int)Math.Pow(2, inStream.ReadStreamPart(2).ConvertToInt());

            inStream.ReadStreamPart(10); // not used

            TotalNumberOfSectorsUsedForSectorAllocationTable = inStream.ReadStreamPart(4).ConvertToInt();

            tempBytes = inStream.ReadStreamPart(4);
            tempInt = tempBytes.ConvertToInt();
            if (tempInt <= 0)
                SpecialValueOfFirstSector = (SpecialSecIds)tempInt;
            else
                SecIDOfFirstSectorOfDirectory = tempInt;

            inStream.ReadStreamPart(4); // not used
            
            MinimumSizeOfStandardStream = inStream.ReadStreamPart(4).ConvertToInt();

            tempBytes = inStream.ReadStreamPart(4); // 12
            tempInt = tempBytes.ConvertToInt();
            if (tempInt <= 0)
                SpecialValueOfFirstSectorOfTheShortSector = (SpecialSecIds)tempInt;
            else
                SecIDOfFirstSectorOfTheShortSector = tempInt;

            TotalNumberOfSectorsUsedForSectorAllocationTableOfTheShortSector = inStream.ReadStreamPart(4).ConvertToInt();


            tempBytes = inStream.ReadStreamPart(4); // 14
            tempInt = tempBytes.ConvertToInt();
            if (tempInt <= 0)
                SpecialValueOfFirstSectorOfTheMasterSectorAllocationTable = (SpecialSecIds)tempInt;
            else
                SecIDOfFirstSectorOfTheMasterSectorAllocationTable = tempInt;

            TotalNumberOfSectorsUsedForSectorAllocationTableForTheMasterSectorAllocationTable = inStream.ReadStreamPart(4).ConvertToInt();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool? _isCompoundHeader;
        /// <summary>
        /// Determines whether [is compound document].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is compound document]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompoundDocument()
        {
            if (_isCompoundHeader.HasValue)
                return _isCompoundHeader.Value;
            return false;
        }

        /// <summary>
        /// Sets the is compound document.
        /// </summary>
        /// <param name="compoundDocumentFileIdentifier">The compound document file identifier.</param>
        /// <returns></returns>
        private bool SetIsCompoundDocument(byte[] compoundDocumentFileIdentifier )
        {
            for (int i = 0; i < compoundDocumentFileIdentifier.Length; i++)
            {
                var headerByte = compoundDocumentFileIdentifier[i];
                if (headerByte != compoundDocumentFileIdentifier[i])
                {
                    _isCompoundHeader = false;
                }
            }
            if (_isCompoundHeader.HasValue == false)
                _isCompoundHeader = true;
            return _isCompoundHeader.Value;
        }
    }
}
