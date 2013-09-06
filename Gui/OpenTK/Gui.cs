using Worldgen.Geom;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;

namespace Worldgen.Gui.OpenTK
{
	public class Gui : GameWindow
	{
		private World world;
		// The world's grid is not going to change, so we can cache '

		public Gui(World world) 
			: base(800, 600, GraphicsMode.Default, "Worldgen")
		{
			this.world = world;
			VSync = VSyncMode.On;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			GL.ClearColor(0, 0, 0, 0);
			GL.Enable(EnableCap.DepthTest);

			GL.ShadeModel(ShadingModel.Flat);
			GL.LightModel(LightModelParameter.LightModelTwoSide, 1.0f);
			GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 1, 1, 1, 0.3f });
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 50.0f);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);

		}

		private void LoadShaders()
		{
			var assem = this.GetType().Assembly;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(ClientRectangle);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);

		
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
			if (Keyboard[Key.Q])
				Exit();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			world.Mtx.WaitOne();
			IList<double> ground = world.GetLayer(World.Height);

			float maxheight = 0;
			foreach (var h in ground) {
				if (h > maxheight)
					maxheight = (float)h;
			}

			Matrix4 modelView = Matrix4.LookAt(Vector3.UnitX * 3, Vector3.Zero, Vector3.UnitZ);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelView);


			GL.Light(LightName.Light0, LightParameter.Position, new float[] { 4f, 1f, 1f, 0f });
			GL.Begin(BeginMode.Triangles);
			GL.Color3(Color.Beige);
			foreach (var face in world.Grid.Faces) {
				for (int i = 0; i < 3; i++)
					GL.Vertex3(world.Grid.Location(face.V[i]) * ground[face.V[i]] / maxheight);
			}
			GL.End();

			world.Mtx.ReleaseMutex();
			SwapBuffers();
		}
	}
}

