using System.IO;
using System.Collections.Generic;

namespace Storage.CompoundDocument
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CompoundDocumentReader
    {
        /// <summary>
        /// Reads the specified in stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <returns></returns>
        public IList<ResultSet> Read(Stream inStream)
        {
            IList<ResultSet> resultsSet = new List<ResultSet>();
            DocumentHeader documentHeader;
            int[] masterSectorAllocationTable;
            int[] sectorAllocationTable;
            int[] shortSectorAllocationTable ;
            IList<Directory> Directories;
            Stream shortStreamContainerStream = null;

            try
            {
                documentHeader = new DocumentHeader(inStream);
                inStream.Position = 76;
                masterSectorAllocationTable = readMasterSectorAllocationTable(inStream, inStream.ReadStreamPart(436), documentHeader.TotalNumberOfSectorsUsedForSectorAllocationTable, documentHeader.SizeOfSector);
                sectorAllocationTable = readSectorAllocationTable(inStream, documentHeader, masterSectorAllocationTable);
                Directories = readDirectoryStorage(inStream, documentHeader, sectorAllocationTable);
                shortSectorAllocationTable = readShortSectorAllocationTable(inStream, documentHeader, sectorAllocationTable);

                foreach (var logicalDir in Directories)
                {
                    if (logicalDir.IsRoot)
                    {
                        shortStreamContainerStream = readShortStreamContainerStream(inStream, documentHeader, sectorAllocationTable, logicalDir);
                        resultsSet.Add(new ResultSet(logicalDir, null));
                        continue;
                    }

                    if (logicalDir.EntryType != TypeOfEntry.UserStream)
                    {
                        resultsSet.Add(new ResultSet(logicalDir, null));
                        continue;
                    }

                    Stream streamResult;
                    if (logicalDir.IsStoredInShortStream)
                        streamResult = readGenericStream(shortStreamContainerStream, logicalDir, shortSectorAllocationTable, documentHeader.SizeOfSectorInShortStream, 0);
                    else
                        streamResult = readGenericStream(inStream, logicalDir, sectorAllocationTable, documentHeader.SizeOfSector, 512);
                    resultsSet.Add(new ResultSet(logicalDir, streamResult));
                }
                return resultsSet;
            }
            finally
            {
                if(shortStreamContainerStream != null)
                {
                    shortStreamContainerStream.Close();
                    shortStreamContainerStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Reads the sector allocation table.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="documentHeader">The document header.</param>
        /// <param name="masterAllocationTable">The master allocation table.</param>
        /// <returns></returns>
        private static int[] readSectorAllocationTable(Stream inStream, DocumentHeader documentHeader, int[] masterAllocationTable)
        {
            var results = new List<int>();
            for (int i = 0; i < documentHeader.TotalNumberOfSectorsUsedForSectorAllocationTable; i++)
            {
                if (i == 0)
                    inStream.Position = 512 + (documentHeader.SecIDOfFirstSectorOfTheMasterSectorAllocationTable * documentHeader.SizeOfSector);
                else
                    inStream.Position = 512 + (masterAllocationTable[i] * documentHeader.SizeOfSector);
                results.AddRange(readSectorChain(documentHeader.SizeOfSector, inStream));
            }
            return results.ToArray();
        }

        /// <summary>
        /// Reads the short sector allocation table.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="documentHeader">The document header.</param>
        /// <param name="sectorAllocationTable">The sector allocation table.</param>
        /// <returns></returns>
        private static int[] readShortSectorAllocationTable(Stream inStream, DocumentHeader documentHeader, int[] sectorAllocationTable)
        {
            var results = new List<int>();
            int valueId = documentHeader.SecIDOfFirstSectorOfTheShortSector;
            while (valueId != -2)
            {
                inStream.Position = 512 + (valueId * documentHeader.SizeOfSector);
                results.AddRange(readSectorChain(documentHeader.SizeOfSector, inStream));
                valueId = sectorAllocationTable[valueId];
            }
            return results.ToArray();
        }

        /// <summary>
        /// Reads the master sector allocation table.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="firstPartOfTheMasterSectorAllocationTable">The first part of the master sector allocation table.</param>
        /// <param name="totalNumberOfSectorsUsedForSectorAllocationTable">The total number of sectors used for sector allocation table.</param>
        /// <param name="sizeOfSector">The size of sector.</param>
        /// <returns></returns>
        private static int[] readMasterSectorAllocationTable(Stream inStream, byte[] firstPartOfTheMasterSectorAllocationTable, int totalNumberOfSectorsUsedForSectorAllocationTable, int sizeOfSector)
        {
            var masterSectorAllocationTable = new List<int>();
            int location = 0;
            Stream masterAllocationBytes = null;
            try
            {
                masterAllocationBytes = new MemoryStream(firstPartOfTheMasterSectorAllocationTable, false);
                masterAllocationBytes.Position = 0;
                for (int i = 0; i < masterAllocationBytes.Length / 4; i++)
                {
                    var sector = masterAllocationBytes.ReadStreamPart(4);
                    location = location + 4;
                    int sectorId;

                    if (i < totalNumberOfSectorsUsedForSectorAllocationTable)
                    {
                        sectorId = sector.ConvertToInt();
                        if (masterAllocationBytes.Length <= masterAllocationBytes.Position)
                        {
                            // read in next sector for master allocation table
                            masterAllocationBytes.Close();
                            masterAllocationBytes.Dispose();
                            inStream.Position = 512 + sizeOfSector * sectorId;
                            masterAllocationBytes = new MemoryStream(inStream.ReadStreamPart(sizeOfSector), true);
                            masterAllocationBytes.Position = 0;
                            location = 0;
                        }
                        else
                        {
                            masterSectorAllocationTable.Add(sectorId);
                        }
                    }
                }
            }
            finally
            {
                if (masterAllocationBytes != null)
                {
                    masterAllocationBytes.Close();
                    masterAllocationBytes.Dispose();
                }
            }
            return masterSectorAllocationTable.ToArray();
        }

        /// <summary>
        /// Reads the directory storage.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="documentHeader">The document header.</param>
        /// <param name="sectorAllocationTable">The sector allocation table.</param>
        /// <returns></returns>
        private static IList<Directory> readDirectoryStorage(Stream inStream, DocumentHeader documentHeader, int[] sectorAllocationTable)
        {
            IList<Directory> Directories = new List<Directory>();
            int valueId;
            valueId = documentHeader.SecIDOfFirstSectorOfDirectory;
            while (valueId != -2)
            {
                inStream.Position = 512 + (valueId * documentHeader.SizeOfSector);
                for (int j = 0; j < documentHeader.SizeOfSector / 128; j++)
                {
                    var dir = new Directory(Directories.Count, inStream);
                    if (dir.TotalStreamSizeInBytes > 0)
                        Directories.Add(dir);
                }
                valueId = sectorAllocationTable[valueId];
            }
            return Directories;
        }

        /// <summary>
        /// Reads the short stream container stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="documentHeader">The document header.</param>
        /// <param name="sectorAllocationTable">The sector allocation table.</param>
        /// <param name="logicalDir">The logical dir.</param>
        /// <returns></returns>
        private static Stream readShortStreamContainerStream(Stream inStream, DocumentHeader documentHeader, int[] sectorAllocationTable, Directory logicalDir)
        {
            var shortStreamContainerStream = new MemoryStream();
            int valueId = logicalDir.SectorIdOfFirstSector;
            int streamSize = logicalDir.TotalStreamSizeInBytes;
            int bytesLeft = streamSize;
            while (valueId != -2)
            {
                int amountToRead = bytesLeft < documentHeader.SizeOfSector ? bytesLeft : documentHeader.SizeOfSector;
                inStream.Position = 512 + (valueId * documentHeader.SizeOfSector);
                shortStreamContainerStream.Write(inStream.ReadStreamPart(amountToRead), 0, amountToRead);
                valueId = sectorAllocationTable[valueId];
                bytesLeft = bytesLeft - amountToRead;
            }
            return shortStreamContainerStream;
        }

        /// <summary>
        /// Reads the generic stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <param name="logicalDir">The logical dir.</param>
        /// <param name="sectorAllocationTable">The sector allocation table.</param>
        /// <param name="sizeOfSector">The size of sector.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private static Stream readGenericStream(Stream inStream, Directory logicalDir, int[] sectorAllocationTable, int sizeOfSector, int offset)
        {
            var outStream = new MemoryStream();

            var sectorId = logicalDir.SectorIdOfFirstSector;
            var streamSize = logicalDir.TotalStreamSizeInBytes;

            int bytesLeft = streamSize;
            int valueId = sectorId;
            while (valueId != -2)
            {
                int amountToRead = bytesLeft < sizeOfSector ? bytesLeft : sizeOfSector;
                inStream.Position = offset + (valueId * sizeOfSector);
                outStream.Write(inStream.ReadStreamPart(amountToRead), 0, amountToRead);
                valueId = sectorAllocationTable[valueId];
                bytesLeft = bytesLeft - amountToRead;
            }
            outStream.Flush();
            outStream.Position = 0;
            return outStream;
        }


        /// <summary>
        /// Reads the sector chain.
        /// </summary>
        /// <param name="sizeOfSector">The size of sector.</param>
        /// <param name="inStream">The in stream.</param>
        /// <returns></returns>
        private static IList<int> readSectorChain(int sizeOfSector, Stream inStream)
        {
            var sectorNumbers = new List<int>();

            bool continueToAdd = true;
            for (int i = 0; i < sizeOfSector / 4; i++)
            {
                var vector = new byte[4];
                inStream.Read(vector, 0, 4);
                var sectorId = vector.ConvertToInt();
                if (continueToAdd)
                {
                    sectorNumbers.Add(sectorId);
                    continueToAdd = sectorId != -1;
                }
            }
            return sectorNumbers;
        }
    }
}
