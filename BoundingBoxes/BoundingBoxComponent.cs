using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace StarWarsArcade.BoundingBoxes
{
	public class BoundingBoxComponent
	{
		private BoundingBoxBuffers _buffers;
		private BasicEffect _effect;
        private BoundingBox boundingBox;
        private Vector3 SizeFromCenter;

		public BoundingBoxComponent(Model MODEL, GraphicsDevice graphicsDevice)
		{
			Model model = MODEL;
			boundingBox = CreateBoundingBox(model);
			_buffers = CreateBoundingBoxBuffers(boundingBox, graphicsDevice);

            SizeFromCenter = (boundingBox.Max - boundingBox.Min) / 2;

			_effect = new BasicEffect(graphicsDevice);
			_effect.LightingEnabled = false;
			_effect.TextureEnabled = false;
			_effect.VertexColorEnabled = true;
		}

        public BoundingBoxComponent(BoundingBox BOX, GraphicsDevice graphicsDevice)
        {
            boundingBox = BOX;
            //_buffers = BUFFER;

            SizeFromCenter = (boundingBox.Max - boundingBox.Min) / 2;
        }

		public void Draw(GraphicsDevice graphicsDevice, Matrix VIEW, Matrix PROJECT)
		{
            _buffers = CreateBoundingBoxBuffers(boundingBox, graphicsDevice);

			graphicsDevice.SetVertexBuffer(_buffers.Vertices);
			graphicsDevice.Indices = _buffers.Indices;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            _effect.World = Matrix.Identity;
			_effect.View = VIEW;
			_effect.Projection = PROJECT;

			foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
					_buffers.VertexCount, 0, _buffers.PrimitiveCount);
			}
		}

        public void Update(Matrix WORLD, Vector3 Tran)
        {
            boundingBox.Max = Tran + SizeFromCenter;
            boundingBox.Min = Tran - SizeFromCenter;
        }

		private BoundingBoxBuffers CreateBoundingBoxBuffers(BoundingBox boundingBox, GraphicsDevice graphicsDevice)
		{
			BoundingBoxBuffers boundingBoxBuffers = new BoundingBoxBuffers();

			boundingBoxBuffers.PrimitiveCount = 24;
			boundingBoxBuffers.VertexCount = 48;

			VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice,
				typeof(VertexPositionColor), boundingBoxBuffers.VertexCount,
				BufferUsage.WriteOnly);
			List<VertexPositionColor> vertices = new List<VertexPositionColor>();

			const float ratio = 5.0f;

			Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0);
			Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0);
			Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio);
			Vector3[] corners = boundingBox.GetCorners();

			// Corner 1.
			AddVertex(vertices, corners[0]);
			AddVertex(vertices, corners[0] + xOffset);
			AddVertex(vertices, corners[0]);
			AddVertex(vertices, corners[0] - yOffset);
			AddVertex(vertices, corners[0]);
			AddVertex(vertices, corners[0] - zOffset);

			// Corner 2.
			AddVertex(vertices, corners[1]);
			AddVertex(vertices, corners[1] - xOffset);
			AddVertex(vertices, corners[1]);
			AddVertex(vertices, corners[1] - yOffset);
			AddVertex(vertices, corners[1]);
			AddVertex(vertices, corners[1] - zOffset);

			// Corner 3.
			AddVertex(vertices, corners[2]);
			AddVertex(vertices, corners[2] - xOffset);
			AddVertex(vertices, corners[2]);
			AddVertex(vertices, corners[2] + yOffset);
			AddVertex(vertices, corners[2]);
			AddVertex(vertices, corners[2] - zOffset);

			// Corner 4.
			AddVertex(vertices, corners[3]);
			AddVertex(vertices, corners[3] + xOffset);
			AddVertex(vertices, corners[3]);
			AddVertex(vertices, corners[3] + yOffset);
			AddVertex(vertices, corners[3]);
			AddVertex(vertices, corners[3] - zOffset);

			// Corner 5.
			AddVertex(vertices, corners[4]);
			AddVertex(vertices, corners[4] + xOffset);
			AddVertex(vertices, corners[4]);
			AddVertex(vertices, corners[4] - yOffset);
			AddVertex(vertices, corners[4]);
			AddVertex(vertices, corners[4] + zOffset);

			// Corner 6.
			AddVertex(vertices, corners[5]);
			AddVertex(vertices, corners[5] - xOffset);
			AddVertex(vertices, corners[5]);
			AddVertex(vertices, corners[5] - yOffset);
			AddVertex(vertices, corners[5]);
			AddVertex(vertices, corners[5] + zOffset);

			// Corner 7.
			AddVertex(vertices, corners[6]);
			AddVertex(vertices, corners[6] - xOffset);
			AddVertex(vertices, corners[6]);
			AddVertex(vertices, corners[6] + yOffset);
			AddVertex(vertices, corners[6]);
			AddVertex(vertices, corners[6] + zOffset);

			// Corner 8.
			AddVertex(vertices, corners[7]);
			AddVertex(vertices, corners[7] + xOffset);
			AddVertex(vertices, corners[7]);
			AddVertex(vertices, corners[7] + yOffset);
			AddVertex(vertices, corners[7]);
			AddVertex(vertices, corners[7] + zOffset);

			vertexBuffer.SetData(vertices.ToArray());
			boundingBoxBuffers.Vertices = vertexBuffer;

			IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, boundingBoxBuffers.VertexCount,
				BufferUsage.WriteOnly);
			indexBuffer.SetData(Enumerable.Range(0, boundingBoxBuffers.VertexCount).Select(i => (short)i).ToArray());
			boundingBoxBuffers.Indices = indexBuffer;

			return boundingBoxBuffers;
		}

		private static void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
		{
			vertices.Add(new VertexPositionColor(position, Color.White));
		}

        public BoundingBox ReturnBox()
        {
            return boundingBox;
        }

        public BoundingBoxBuffers ReturnBuffers()
        {
            return _buffers;
        }

		private static BoundingBox CreateBoundingBox(Model model)
		{
			Matrix[] boneTransforms = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(boneTransforms);

			BoundingBox result = new BoundingBox();
			foreach (ModelMesh mesh in model.Meshes)
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					BoundingBox? meshPartBoundingBox = GetBoundingBox(meshPart, boneTransforms[mesh.ParentBone.Index]);
					if (meshPartBoundingBox != null)
						result = BoundingBox.CreateMerged(result, meshPartBoundingBox.Value);
				}
			return result;
		}

		private static BoundingBox? GetBoundingBox(ModelMeshPart meshPart, Matrix transform)
		{
			if (meshPart.VertexBuffer == null)
				return null;

			Vector3[] positions = VertexElementExtractor.GetVertexElement(meshPart, VertexElementUsage.Position);
			if (positions == null)
				return null;

			Vector3[] transformedPositions = new Vector3[positions.Length];
			Vector3.Transform(positions, ref transform, transformedPositions);

			return BoundingBox.CreateFromPoints(transformedPositions);
		}
	}
}