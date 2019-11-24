using Annex.Graphics;
using Game.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game.Models.Chunks
{
    public class Map : IDrawableObject
    {
        private Dictionary<(int x, int y), MapChunk> _chunks;
        private HashSet<Entity> _mapEntities;
        public readonly string Name;
        public string Folder => $"resources/maps/{Name}/";
        public string GetChunkFile(int x, int y) => Folder + $"({x})({y}).json";

        public Map(string name) {
            this.Name = name;
            this._chunks = new Dictionary<(int, int), MapChunk>();
            this._mapEntities = new HashSet<Entity>();
        }

        public MapChunk GetChunk(int x, int y) {
            LoadChunk(x, y);
            return this._chunks[(x, y)];
        }

        public void Draw(ICanvas canvas) {
            foreach (var value in this._chunks.Values) {
                value.Draw(canvas);
            }
            foreach (var entity in this.GetOrderedEntities()) {
                entity.Draw(canvas);
            }
        }

        internal void Save() {
            Directory.CreateDirectory(Folder);
            foreach (var entry in this._chunks) {
                Json.SaveChunk(GetChunkFile(entry.Key.x, entry.Key.y), entry.Value);
            }
        }

        public void LoadChunk(int x, int y) {
            if (!this._chunks.ContainsKey((x, y))) {
                this._chunks[(x, y)] = Json.LoadChunk(GetChunkFile(x, y)) ?? new MapChunk(x, y);
            }
        }

        public void AddEntity(Entity e) {
            this._mapEntities.Add(e);
        }

        public IEnumerable<Entity> GetOrderedEntities() {
            return this._mapEntities.OrderBy(e => e.Position.Y);
        }

        public void RemoveEntity(Entity e) {
            this._mapEntities.Remove(e);
        }

        public void UnloadChunk(int currentPlayerX, int currentPlayerY, bool isChunkEdited = false) {
            const int offSetConstant = 1;
            HashSet<(int, int)> chunkAroundPlayer = new HashSet<(int, int)>();

            //TODO: to refactor the unload chunks
            //foreach player:
            //    for x= -1 to 1
            //        for y = -1 to 1:
            //            chunkAroundPlayer.Add(Player.x + x )

            //add current player position
            chunkAroundPlayer.Add((currentPlayerX, currentPlayerY));
            //check north
            if (this._chunks.ContainsKey((currentPlayerX, currentPlayerY + offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX, currentPlayerY + offSetConstant));
            //check west
            if (this._chunks.ContainsKey((currentPlayerX - offSetConstant, currentPlayerY)))
                chunkAroundPlayer.Add((currentPlayerX - offSetConstant, currentPlayerY));
            //check south
            if (this._chunks.ContainsKey((currentPlayerX, currentPlayerY - offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX, currentPlayerY - offSetConstant));
            //check east
            if (this._chunks.ContainsKey((currentPlayerX + offSetConstant, currentPlayerY)))
                chunkAroundPlayer.Add((currentPlayerX + offSetConstant, currentPlayerY));
            //check west-north
            if (this._chunks.ContainsKey((currentPlayerX - offSetConstant, currentPlayerY + offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX - offSetConstant, currentPlayerY + offSetConstant));
            //check west-south
            if (this._chunks.ContainsKey((currentPlayerX - offSetConstant, currentPlayerY - offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX - offSetConstant, currentPlayerY - offSetConstant));
            //check south-east
            if (this._chunks.ContainsKey((currentPlayerX + offSetConstant, currentPlayerY - offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX + offSetConstant, currentPlayerY - offSetConstant));
            //check north-east
            if (this._chunks.ContainsKey((currentPlayerX + offSetConstant, currentPlayerY + offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX + offSetConstant, currentPlayerY + offSetConstant));

            //remove all chunk that are not in the hashset
            foreach (var entry in this._chunks) {
                if (chunkAroundPlayer.Contains(entry.Key)) {
                    continue;
                }
                this._chunks.Remove((entry.Value.X, entry.Value.Y));
            }
        }

        public (float x, float y) GetMaximumColllisions(HitboxEntity entity) {
            float maxX = 0;
            float maxY = 0;
            foreach (var otherEntity in this._mapEntities) {
                if (otherEntity == entity) {
                    continue;
                }
                var other = otherEntity as HitboxEntity;
                if (other == null) {
                    continue;
                }

                float x = 0;
                float y = 0;
                bool xCollision = false;
                bool yCollision = false;

                // Border collisions.
                if (entity.RealLeft > other.RealLeft && entity.RealLeft < other.RealRight) {
                    x = other.RealRight - entity.RealLeft;
                    xCollision = true;
                }
                if (entity.RealRight > other.RealLeft && entity.RealRight < other.RealRight) {
                    x = other.RealLeft - entity.RealRight;
                    xCollision = true;
                }
                if (entity.RealTop < other.RealBottom && entity.RealTop > other.RealTop) {
                    y = other.RealBottom - entity.RealTop;
                    yCollision = true;
                }
                if (entity.RealBottom > other.RealTop && entity.RealBottom < other.RealBottom) {
                    y = other.RealTop - entity.RealBottom;
                    yCollision = true;
                }

                // Is the other inside our hitbox?
                if (!(yCollision || xCollision)) {
                    if (entity.RealLeft < other.RealLeft && entity.RealRight > other.RealRight) {
                        x = -1;
                        xCollision = true;
                    }
                    if (entity.RealTop < other.RealTop && entity.RealBottom > other.RealBottom) {
                        y = -1;
                        yCollision = true;
                    }
                }

                // Swallowing collisions
                if (yCollision && !xCollision && entity.RealLeft < other.RealLeft && entity.RealRight > other.RealRight) {
                    xCollision = true;
                }
                if (xCollision && !yCollision && entity.RealTop < other.RealTop && entity.RealBottom > other.RealBottom) {
                    yCollision = true;
                }

                if (xCollision && yCollision) {
                    if (Math.Abs(x) > Math.Abs(maxX)) {
                        maxX = x;
                    }
                    if (Math.Abs(y) > Math.Abs(maxY)) {
                        maxY = y;
                    }
                    entity.OnCollision(other);
                    other.OnCollision(entity);
                }
            }

            if (Math.Abs(maxX) < Math.Abs(maxY)) {
                maxY = 0;
            } else {
                maxX = 0;
            }
            return (maxX, maxY);
        }
    }
}
