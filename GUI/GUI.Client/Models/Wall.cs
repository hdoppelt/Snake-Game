﻿using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     TODO: XML COMMENT
    /// </summary>
    public class Wall
    {
        /// <summary>
        ///     Unique ID of the wall.
        /// </summary>
        [JsonInclude]
        public int wall { get; set; }

        /// <summary>
        ///     One endpoint of the wall.
        /// </summary>
        [JsonInclude]
        public Point2D p1 { get; set; }

        /// <summary>
        ///     The other endpoint of the wall.
        /// </summary>
        [JsonInclude]
        public Point2D p2 { get; set; }

        /// <summary>
        ///     Default constructor for JSON deserialization.
        /// </summary>
        public Wall()
        {
            p1 = new Point2D();
            p2 = new Point2D();
        }
    }
}