﻿using System.Drawing;
using System.Numerics;

namespace GUI.Client.Models
{
    /// <summary>
    ///     Represents the game world containing snakes, walls, and powerups.
    /// </summary>
    public class World
    {
        /// <summary>
        ///     Collection of all snakes in the world, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Snake> Snakes { get; set; }

        /// <summary>
        ///     Collection of all walls in the world, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Wall> Walls { get; set; }

        /// <summary>
        ///     Collection of all powerups in the world, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Powerup> Powerups { get; set; }

        /// <summary>
        ///     Property that gets or sets the size of the world (width and height).
        /// </summary>
        public int WorldSize { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="World"/> class with the specified world size.
        /// </summary>
        /// <param name="worldSize">The size of the world.</param>
        public World(int worldSize)
        {
            this.WorldSize = worldSize;
            this.Snakes = new Dictionary<int, Snake>();
            this.Walls = new Dictionary<int, Wall>();
            this.Powerups = new Dictionary<int, Powerup>();
        }

        /// <summary>
        ///     Shallow copy constructor.
        /// </summary>
        /// <param name="world"></param>
        public World(World world)
        {
            Walls = new(world.Walls);
            Snakes = new(world.Snakes);
            Powerups = new(world.Powerups);
            WorldSize = world.WorldSize;
        }
    }
}