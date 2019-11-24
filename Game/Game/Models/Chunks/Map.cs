using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models.Chunks
{
    public class Map : IDrawableObject
    {
        private TextureContext _background;
        public const int Size_X = 500;
        public const int Size_Y = 500;
        private HashSet<Entity> _mapEntities;
        public readonly string Name;

        public Map(string name) {
            this.Name = name;
            this._mapEntities = new HashSet<Entity>();
            this._background = new TextureContext("Grassy-Tiles.png") {
                SourceTextureRect = new IntRect(64, 64, 32, 32),
                RenderSize = Vector.Create(Size_X, Size_Y)
            };
        }

        public void Draw(ICanvas canvas) {
            canvas.Draw(this._background);
            foreach (var entity in this.GetOrderedEntities()) {
                entity.Draw(canvas);
            }
        }

        public IEnumerable<Entity> GetEntities(Func<Entity, bool> cmp = null)
        {
            if (cmp == null)
            {
                return this._mapEntities;
            }
            return this._mapEntities.Where(cmp);
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

        public (float x, float y) GetMaximumColllisions(HitboxEntity entity) {
            float maxX = 0;
            float maxY = 0;

            if (entity.RealLeft < 0) {
                maxX = -entity.RealLeft;
            }
            if (entity.RealTop < 0) {
                maxY = -entity.RealTop;
            }
            if (entity.RealRight > Map.Size_X) {
                maxX = -(entity.RealRight - Map.Size_X);
            }
            if (entity.RealBottom > Map.Size_Y) {
                maxY = -(entity.Position.Y - Map.Size_Y);
            }

            if (maxX != 0 || maxY != 0) {
                return (maxX, maxY);
            }

            var entities = this._mapEntities.ToList();
            for (int i = 0; i < entities.Count; i++)
            {
                var otherEntity = entities[i];
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
