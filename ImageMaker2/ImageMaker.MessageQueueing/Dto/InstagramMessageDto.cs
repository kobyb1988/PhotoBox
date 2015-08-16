using System;

namespace ImageMaker.MessageQueueing.Dto
{
    public class InstagramMessageDto
    {
        public DateTime TransferTime { get; set; }

        public byte[] Data { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Name { get; set; }
    }
}
