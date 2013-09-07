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
		private int basemapVtxBuffer, heightBuffer, waterBuffer, basemapFaceBuffer;
		private float[,] basemapVtxData;
		private uint[,] basemapFaceData;
		private float[] heightData, waterData;
		private int glslProg;
		private Quaternion viewAngle;
		private bool errp = false;
		// The world's grid is not going to change, so we can cache '

		public Gui(World world)
			: base(800, 600, GraphicsMode.Default, "Worldgen")
		{
			this.world = world;
			VSync = VSyncMode.On;
			viewAngle = Quaternion.Identity;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.LoadShaders(false);


			GL.ClearColor(0, 0, 0, 0);
			GL.Enable(EnableCap.DepthTest);

#if false
			GL.ShadeModel(ShadingModel.Flat);
			GL.LightModel(LightModelParameter.LightModelTwoSide, 1.0f);
			GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 1, 1, 1, 0.3f });
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 50.0f);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);

#endif

			// create data arrays...
			GL.GenBuffers(1, out basemapVtxBuffer);
			GL.GenBuffers(1, out basemapFaceBuffer);
			GL.GenBuffers(1, out heightBuffer);
			GL.GenBuffers(1, out waterBuffer);

			GL.BindBuffer(BufferTarget.ArrayBuffer, basemapVtxBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, basemapFaceBuffer);

			basemapVtxData = new float[world.Grid.Count, 6];
			for (int i = 0; i < world.Grid.Count; i++) {
				Point3d pt = world.Grid.Location(i);
				basemapVtxData[i, 0] = (float)pt.X;
				basemapVtxData[i, 1] = (float)pt.Y;
				basemapVtxData[i, 2] = (float)pt.Z;
			}

			List<Face> faces = new List<Face>(world.Grid.Faces);
			basemapFaceData = new uint[faces.Count, 3];
			for (int i = 0; i < faces.Count; i++) {
				var face = faces[i];
				var gr = world.Grid;
				var normal = Vector3.Cross(gr.Location(face.V[1]) - gr.Location(face.V[0]),
				                           gr.Location(face.V[2]) - gr.Location(face.V[0]));
				if (Vector3.Dot(normal, gr.Location(face.V[0])) < 0) { // BUG: this might be inverted.
					//if ((i % 2) == 0) {
					// outward-facing, use vertices as given
					basemapFaceData[i, 0] = (uint)face.V[0];
					basemapFaceData[i, 1] = (uint)face.V[1];
					basemapFaceData[i, 2] = (uint)face.V[2];
				} else {
					basemapFaceData[i, 0] = (uint)face.V[2];
					basemapFaceData[i, 1] = (uint)face.V[1];
					basemapFaceData[i, 2] = (uint)face.V[0];
				}
			}

			GL.BufferData(BufferTarget.ArrayBuffer,
			              (IntPtr)(basemapVtxData.Length * sizeof(float)),
			              basemapVtxData,
			              BufferUsageHint.DynamicDraw);
			GL.BufferData(BufferTarget.ElementArrayBuffer, 
			              (IntPtr)(basemapFaceData.Length * sizeof(int)), 
			              basemapFaceData, 
			              BufferUsageHint.DynamicDraw);

			heightData = new float[world.Grid.Count];
			waterData = new float[world.Grid.Count];
			GL.BindBuffer(BufferTarget.ArrayBuffer, heightBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer,
			              (IntPtr)(heightData.Length * sizeof(float)),
			              heightData,
			              BufferUsageHint.DynamicDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, waterBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer,
			              (IntPtr)(waterData.Length * sizeof(float)),
			              waterData,
			              BufferUsageHint.DynamicDraw);

		}

		private void LoadShaders(bool from_fs)
		{
			string fragSource;
			string vtxSource;
			if (from_fs) {
				vtxSource = System.IO.File.ReadAllText("../../Gui/OpenTK/vertex.glsl");
				fragSource = System.IO.File.ReadAllText("../../Gui/OpenTK/fragment.glsl");
			} else {
				fragSource = readResource(this.GetType(), "fragment.glsl");
				vtxSource = readResource(this.GetType(), "vertex.glsl");
			}
			bool hadError = false;
			var vtx = GL.CreateShader(ShaderType.VertexShader);
			var frag = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(vtx, vtxSource);
			GL.ShaderSource(frag, fragSource);
			GL.CompileShader(vtx);
			GL.CompileShader(frag);
			int compileStatus;

			GL.GetShader(vtx, ShaderParameter.CompileStatus, out compileStatus);
			if (compileStatus == 0) {
				Console.Error.WriteLine("Error compiling vertex shader:\n{0}", GL.GetShaderInfoLog(vtx));
				Console.Error.Flush();
				hadError = true;
			}
			GL.GetShader(frag, ShaderParameter.CompileStatus, out compileStatus);
			if (compileStatus == 0) {
				Console.Error.WriteLine("Error compiling fragment shader:\n{0}", GL.GetShaderInfoLog(frag));
				Console.Error.Flush();
				hadError = true;
			}

			if (hadError)
				goto fail;
			int prog = GL.CreateProgram();
			GL.AttachShader(prog, vtx);
			GL.AttachShader(prog, frag);

			GL.LinkProgram(prog);
			GL.GetProgram(prog, ProgramParameter.LinkStatus, out compileStatus);
			if (compileStatus == 0) {
				Console.Error.WriteLine("Error linking shaders:\n{0}", GL.GetProgramInfoLog(prog));
				Console.Error.Flush();
				goto fail;
			}
			glslProg = prog;
			GL.UseProgram(prog);
			GL.DeleteShader(vtx);
			GL.DeleteShader(frag);

			int numParams;
			GL.GetProgram(prog, ProgramParameter.ActiveAttributes, out numParams);
			errp = false;
			return;
			fail:
			if (!from_fs)
				System.Environment.Exit(1);
			errp = true;
		}

		private string readResource(Type type, string name)
		{
			var rsrc = this.GetType().Assembly.GetManifestResourceStream(type, name);
			var buf = new byte[rsrc.Length];
			rsrc.Read(buf, 0, (int)rsrc.Length);

			return new System.Text.UTF8Encoding(false, true).GetString(buf);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(ClientRectangle);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			const float rotAngle = (float)Math.PI / 100;
			base.OnUpdateFrame(e);
			if (Keyboard[Key.Q]) {
				Exit();
				System.Environment.Exit(0);
			} else if (Keyboard[Key.R]) {
				LoadShaders(true);
			}

			if (Keyboard[Key.Right])
				viewAngle = Quaternion.FromAxisAngle(Vector3.UnitZ, rotAngle) * viewAngle;
			if (Keyboard[Key.Left])
				viewAngle = Quaternion.FromAxisAngle(Vector3.UnitZ, -rotAngle) * viewAngle;
			if (Keyboard[Key.Up])
				viewAngle = Quaternion.FromAxisAngle(Vector3.UnitY, -rotAngle) * viewAngle;
			if (Keyboard[Key.Down])
				viewAngle = Quaternion.FromAxisAngle(Vector3.UnitY, rotAngle) * viewAngle;
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);

			GL.UniformMatrix4(GL.GetUniformLocation(glslProg,"P"), false, ref projection);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//world.Mtx.WaitOne();
			IList<double> ground = world.GetLayer(World.Height);

			float maxheight = 0;
			foreach (var h in ground) {
				if (h > maxheight)
					maxheight = (float)h;
			}


			Matrix4 view = Matrix4.LookAt(Vector3.UnitX * 3, Vector3.Zero, Vector3.UnitZ);
			Matrix4 model = Matrix4.Rotate(viewAngle);
			GL.UniformMatrix4(GL.GetUniformLocation(glslProg, "V"), false, ref view);
			GL.UniformMatrix4(GL.GetUniformLocation(glslProg, "M"), false, ref model);
			if (GL.GetUniformLocation(glslProg, "errp") != -1) {
				GL.Uniform1(GL.GetUniformLocation(glslProg, "errp"), errp ? 1 : 0);
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, basemapVtxBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, basemapFaceBuffer);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(GL.GetAttribLocation(glslProg, "vertex_m"),
			                       3, VertexAttribPointerType.Float, false, 24, 0);
			/* GL.VertexAttribPointer(GL.GetAttribLocation(glslProg, "normal_m"),
			                       3, VertexAttribPointerType.Float, false, 24, 12); */

			var lightPos = new Vector3(6, 1, 1);
			GL.Uniform3(GL.GetUniformLocation(glslProg, "lightPos_w"),
			            ref lightPos);

			for (int i = 0; i < ground.Count; i++) {
				heightData[i] = (float)ground[i];
			}
			GL.BindBuffer(BufferTarget.ArrayBuffer, heightBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer,
			              (IntPtr)(heightData.Length * sizeof(float)),
			              heightData,
			              BufferUsageHint.DynamicDraw);
			GL.EnableVertexAttribArray(GL.GetAttribLocation(glslProg, "height"));
			GL.VertexAttribPointer(GL.GetAttribLocation(glslProg, "height"),
			                       1, VertexAttribPointerType.Float, false, 0, 0);
			GL.Uniform1(GL.GetUniformLocation(glslProg, "maxheight"), maxheight);
			GL.DrawElements(BeginMode.Triangles, basemapFaceData.Length, DrawElementsType.UnsignedInt, 0);

			SwapBuffers();
		}
	}
}