using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;

public class Chip8 : MonoBehaviour {
	public UInt16 opcode;
	public byte[] memory = new byte[4096];
	public byte[] SV = new byte[8];
	public byte[] V = new byte[16];
	public UInt16 I;
	public UInt16 pc;
	public Int16 delay_timer;
	public Int16 sound_timer;
	public UInt16[] stack = new UInt16[16];
	public byte sp;
	public byte[] keys = new byte[16];
	public bool drawFlag;

	public bool extendedScreen;

	byte[] fontset = {
		0xF0, 0x90, 0x90, 0x90, 0xF0, //0
		0x20, 0x60, 0x20, 0x20, 0x70, //1
		0xF0, 0x10, 0xF0, 0x80, 0xF0, //2
		0xF0, 0x10, 0xF0, 0x10, 0xF0, //3
		0x90, 0x90, 0xF0, 0x10, 0x10, //4
		0xF0, 0x80, 0xF0, 0x10, 0xF0, //5
		0xF0, 0x80, 0xF0, 0x90, 0xF0, //6
		0xF0, 0x10, 0x20, 0x40, 0x40, //7
		0xF0, 0x90, 0xF0, 0x90, 0xF0, //8
		0xF0, 0x90, 0xF0, 0x10, 0xF0, //9
		0xF0, 0x90, 0xF0, 0x90, 0x90, //A
		0xE0, 0x90, 0xE0, 0x90, 0xE0, //B
		0xF0, 0x80, 0x80, 0x80, 0xF0, //C
		0xE0, 0x90, 0x90, 0x90, 0xE0, //D
		0xF0, 0x80, 0xF0, 0x80, 0xF0, //E
		0xF0, 0x80, 0xF0, 0x80, 0x80  //F
	};

	UInt16[] fontset_hq = {
		0x7C, 0xC6, 0xCE, 0xDE, 0xD6, 0xF6, 0xE6, 0xC6, 0x7C, 0x00, // 0
		0x10, 0x30, 0xF0, 0x30, 0x30, 0x30, 0x30, 0x30, 0xFC, 0x00, // 1
		0x78, 0xCC, 0xCC, 0x0C, 0x18, 0x30, 0x60, 0xCC, 0xFC, 0x00, // 2
		0x78, 0xCC, 0x0C, 0x0C, 0x38, 0x0C, 0x0C, 0xCC, 0x78, 0x00, // 3
		0x0C, 0x1C, 0x3C, 0x6C, 0xCC, 0xFE, 0x0C, 0x0C, 0x1E, 0x00, // 4
		0xFC, 0xC0, 0xC0, 0xC0, 0xF8, 0x0C, 0x0C, 0xCC, 0x78, 0x00, // 5
		0x38, 0x60, 0xC0, 0xC0, 0xF8, 0xCC, 0xCC, 0xCC, 0x78, 0x00, // 6
		0xFE, 0xC6, 0xC6, 0x06, 0x0C, 0x18, 0x30, 0x30, 0x30, 0x00, // 7
		0x78, 0xCC, 0xCC, 0xEC, 0x78, 0xDC, 0xCC, 0xCC, 0x78, 0x00, // 8
		0x7C, 0xC6, 0xC6, 0xC6, 0x7E, 0x0C, 0x18, 0x30, 0x70, 0x00, // 9
		0x30, 0x78, 0xCC, 0xCC, 0xCC, 0xFC, 0xCC, 0xCC, 0xCC, 0x00, // A
		0xFC, 0x66, 0x66, 0x66, 0x7C, 0x66, 0x66, 0x66, 0xFC, 0x00, // B
		0x3C, 0x66, 0xC6, 0xC0, 0xC0, 0xC0, 0xC6, 0x66, 0x3C, 0x00, // C
		0xF8, 0x6C, 0x66, 0x66, 0x66, 0x66, 0x66, 0x6C, 0xF8, 0x00, // D
		0xFE, 0x62, 0x60, 0x64, 0x7C, 0x64, 0x60, 0x62, 0xFE, 0x00, // E
		0xFE, 0x66, 0x62, 0x64, 0x7C, 0x64, 0x60, 0x60, 0xF0, 0x00  // F
	};

	public byte screen_width;
	public byte screen_height;

	public uint[] canvas = new uint[2048];
	uint[] oldCanvas = new uint[2048];

	public bool started = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void initialize() {
		opcode = 0;
		I = 0;
		sp = 0;
		delay_timer = 0;
		sound_timer = 0;
		pc = 0x200;

		extendedScreen = false;
		screen_width = 64;
		screen_height = 32;

		for (int i = 0; i < canvas.Length; i++) canvas[i] = 0;
		for (int i = 0; i < keys.Length; i++) keys[i] = 0;
		for (int i = 0; i < stack.Length; i++) stack[i] = 0;
		for (int i = 0; i < V.Length; i++) V[i] = 0;
		for (int i = 0; i < SV.Length; i++) SV[i] = 0;
		for (int i = 0; i < memory.Length; i++) memory[i] = 0;

		for (int i = 0; i < fontset.Length; i++) memory[i] = (byte)fontset[i];
		for (int i = 0; i < fontset_hq.Length; i++) memory[i + fontset.Length] = (byte)fontset_hq[i];
	}

	public void loadProgram(string filePath) {
		started = false;
		byte[] fileData = File.ReadAllBytes(filePath);

		loadProgram(fileData);
	}

	public void loadProgram(byte[] fileData) {
		started = false;
		initialize();

		if (3584 > fileData.Length) {
			for (int i = 0; i < fileData.Length; i++) {
				memory[i + 0x200] = fileData[i];
			}
		} else {
			Debug.LogError("File size too big!");
		}
		started = true;
	}

	void setKeys() {
		for (int i = 0; i < keys.Length; i++) {
			keys[i] = Input.GetButton(i.ToString()) ? (byte)0xff : (byte)0;
		}
	}

	public void emulateCycle(int steps) {
		setKeys();

		for (int step = 0; step < steps; step++) {
			byte x;
			byte y;
			opcode = (UInt16)((memory[pc] << 8) | memory[pc + 1]);
			x = (byte)((opcode & 0x0f00) >> 8);
			y = (byte)((opcode & 0x00f0) >> 4);

			pc += 2;

			switch (opcode & 0xf000) {
				case 0x0000: {
						switch (opcode & 0x00f0) {
							case 0x00c0: { //SCD
									byte n = (byte)(opcode & 0x000f);

									/*for (int yd = screen_height-1; yd > n; yd--) {
										for (int xd = 0; xd < screen_width; xd++) {
											canvas[yd * screen_width + xd] = canvas[(yd-n) * screen_width + xd];
										}
									}

									for (int yd = 0; yd < n; yd++) {
										for (int xd = 0; xd < screen_width; xd++) {
											canvas[yd * screen_width + xd] = 0;
										}
									}*/

									for (int i = screen_height - 2; i >= 0; i--) {
										for (int j = 0; j < screen_width; j++) {
											canvas[(i + n) * screen_width + j] = canvas[i * screen_width + j];
										}
										for (int j = 0; j < screen_width; j++) {
											canvas[j + i * screen_width] = 0;
										}
									}

									drawFlag = true;

									break;
								}
								break;
						}
						switch (opcode & 0x00ff) {
							case 0x00e0:
								for (int i = 0; i < canvas.Length; i++) canvas[i] = 0;
								drawFlag = true;
								break;
							case 0x00ee:
								pc = stack[(--sp) & 0xf];
								break;
							case 0x00fb: { //SCR
									for (int yd = 0; yd < screen_height; yd++) {
										for (int xd = screen_width-1; xd > 3; xd--) {
											canvas[yd * screen_width + xd] = canvas[yd * screen_width + (xd - 4)];
										}
										canvas[yd * screen_width] = 0;
										canvas[yd * screen_width + 1] = 0;
										canvas[yd * screen_width + 2] = 0;
										canvas[yd * screen_width + 3] = 0;
									}
									/*for (int i = 0; i < screen_height; i++) {
										for (int j = 0; j < screen_width - 4; j++) {
											canvas[i*screen_width + j + 4] = canvas[i * screen_width + j];
										}
										for (int j = 0; j < 4; j++) canvas[i * screen_width + j] = 0;
									}*/
									break;
								}
							case 0x00fc: { //SCL
									for (int yd = 0; yd < screen_height; yd++) {
										for (int xd = 0; xd < screen_width-4; xd++) {
											canvas[yd * screen_width + xd] = canvas[yd * screen_width + (xd + 4)];
										}
										canvas[yd * screen_width + 124] = 0;
										canvas[yd * screen_width + 125] = 0;
										canvas[yd * screen_width + 126] = 0;
										canvas[yd * screen_width + 127] = 0;
									}
									/*for (int i = 0; i < screen_height; i++) {
										for (int j = 0; j < screen_width - 4; j++) {
											canvas[i * screen_width + j] = canvas[i * screen_width + j + 4];
										}
										for (int j = screen_width - 4; j < screen_width; j++) canvas[i * screen_width + j] = 0;
									}*/
									break;
								}
							case 0x00fd:
								exitGame();
								//exit
								break;
							case 0x00fe:
								extendedScreen = false;
								screen_width = 64;
								screen_height = 32;
								oldCanvas = new uint[8192];
								for (int i = 0; i < canvas.Length; i++) {
									oldCanvas[i] = canvas[i];
								}
								canvas = new uint[2048];
								for (int i = 0; i < canvas.Length; i++) {
									canvas[i] = oldCanvas[i];
								}
								if (GetComponent<Main>() != null) GetComponent<Main>().changeResolution(screen_width);
								else if (GetComponent<TSMain>() != null) GetComponent<TSMain>().changeResolution(screen_width);
								break;
							case 0x00ff:
								extendedScreen = true;
								screen_width = 128;
								screen_height = 64;
								oldCanvas = new uint[2048];
								for (int i = 0; i < oldCanvas.Length; i++) {
									oldCanvas[i] = canvas[i];
								}
								canvas = new uint[8192];
								for (int i = 0; i < oldCanvas.Length; i++) {
									canvas[i] = oldCanvas[i];
								}
								if (GetComponent<Main>() != null) GetComponent<Main>().changeResolution(screen_width);
								else if (GetComponent<TSMain>() != null) GetComponent<TSMain>().changeResolution(screen_width);
								break;
							default:
								pc = (UInt16)(pc & 0x0fff);
								break;
						}
						break;
					}
				case 0x1000: {
						pc = (UInt16)(opcode & 0x0fff);
						break;
					}
				case 0x2000: {
						stack[sp++] = pc;
						pc = (UInt16)(opcode & 0x0fff);
						break;
					}
				case 0x3000: {
						if (V[x] == (opcode & 0x00ff))
							pc += 2;
						break;
					}
				case 0x4000: {
						if (V[x] != (opcode & 0x00ff))
							pc += 2;
						break;
					}
				case 0x5000: {
						if (V[x] == V[y])
							pc += 2;
						break;
					}
				case 0x6000: {
						V[x] = (byte)(opcode & 0x00ff);
						break;
					}
				case 0x7000: {
						V[x] = (byte)((V[x] + (opcode & 0x00ff)) & 0xff);
						break;
					}
				case 0x8000: {
						switch (opcode & 0x000f) {
							case 0x0000: {
									V[x] = V[y];
									break;
								}
							case 0x0001: {
									V[x] |= V[y];
									break;
								}
							case 0x0002: {
									V[x] &= V[y];
									break;
								}
							case 0x0003: {
									V[x] ^= V[y];
									break;
								}
							case 0x0004: {
									V[0xf] = (V[x] + V[y] > 0xff) ? (byte)1 : (byte)0;
									V[x] += V[y];
									V[x] &= 255;
									break;
								}
							case 0x0005: {
									V[0xf] = (V[x] >= V[y]) ? (byte)1 : (byte)0;
									V[x] -= V[y];
									break;
								}
							case 0x0006: {
									V[0xf] = (byte)(V[x] & 1);
									V[x] >>= 1;
									break;
								}
							case 0x0007: {
									V[0xf] = (V[y] >= V[x]) ? (byte)1 : (byte)0;
									V[x] = (byte)(V[y] - V[x]);
									break;
								}
							case 0x000E: {
									V[0xf] = (byte)(V[x] >> 7);
									V[x] <<= 1;
									break;
								}
								break;
						}
						break;
					}
				case 0x9000: {
						if (V[x] != V[y])
							pc += 2;
						break;
					}
				case 0xa000: {
						I = (UInt16)(opcode & 0x0fff);
						break;
					}
				case 0xb000: {
						pc = (UInt16)(V[0] + (opcode & 0x0fff));
						break;
					}
				case 0xc000: {
						V[x] = (byte)((UnityEngine.Random.Range(0x00, 0xff)) & (opcode & 0x00FF));
						break;
					}
				case 0xd000: {
						uint xd = V[x];
						uint yd = V[y];
						uint height = (UInt16)(opcode & 0x000f);

						V[0xf] = 0;

						if (extendedScreen) {
							//Extended screen DXY0
							uint cols = 1;
							if (height == 0) {
								cols = 2;
								height = 16;
							}
							for (int _y = 0; _y < height; ++_y) {
								uint pixel = memory[I + (cols * _y)];
								if (cols == 2) {
									pixel <<= 8;
									pixel |= memory[I + (_y << 1) + 1];
								}
								for (int _x = 0; _x < (cols << 3); ++_x) {
									if ((pixel & (((cols == 2) ? 0x8000 : 0x80) >> _x)) != 0) {
										int index = (int)(((xd + _x) & 0x7f) + (((yd + _y) & 0x3f) << 7));
										V[0xf] |= (byte)(canvas[index] & 1);
										if (canvas[index] == 0xff)
											canvas[index] = 0;
										else
											canvas[index] = 0xff;
									}
								}
							}
						} else {
							//Normal screen DXYN
							if (height == 0) height = 16;
							for (int _y = 0; _y < height; ++_y) {
								uint pixel = memory[I + _y];
								for (int _x = 0; _x < 8; ++_x) {
									if ((pixel & (0x80 >> _x)) != 0) {
										int index = (int)(((xd + _x) & 0x3f) + (((yd + _y) & 0x1f) << 6));
										V[0xf] |= (byte)(canvas[index] & 1);
										if (canvas[index] == 0xff)
											canvas[index] = 0;
										else
											canvas[index] = 0xff;
									}
								}
							}
						}

						drawFlag = true;

						break;
					}
				case 0xe000: {
						switch (opcode & 0x00ff) {
							case 0x009e: {
									if (keys[V[x]] == 0xff)
										pc += 2;
									break;
								}
							case 0x00a1: {
									if (keys[V[x]] == 0x00)
										pc += 2;
									break;
								}
						}
						break;
					}
				case 0xf000: {
						switch (opcode & 0x00ff) {
							case 0x0007: {
								V[x] = (byte)delay_timer;
								break;
							}
							case 0x000A: {
								bool key_pressed = false;
								pc -= 2;

								for (int i = 0; i < 16; ++i) {
									if (keys[i] == 0xff) {
										V[x] = (byte)i;
										pc += 2;
										key_pressed = true;
										break;
									}
								}

								if (!key_pressed)
									return;
								break;
							}
							case 0x0015: {
								delay_timer = V[x];
								break;
							}
							case 0x0018: {
								sound_timer = V[x];
								break;
							}
							case 0x001E: {
								I = (UInt16)((I + V[x]) & 0xffff);
								break;
							}
							case 0x0029: {
								I = (UInt16)((V[x] & 0xf) * 5);
								break;
							}
							case 0x0030: {
								I = (UInt16)((V[x] & 0xf) * 10 + 80);
								break;
							}
							case 0x0033: {
								memory[I] = (byte)(V[x] / 100);
								memory[I + 1] = (byte)((V[x] / 10) % 10);
								memory[I + 2] = (byte)(V[x] % 10);
								break;
							}
							case 0x0055: {
								for (int i = 0; i <= x; ++i) {
									memory[I + i] = V[i];
								}
								//I += (byte)(x + 1);
								break;
							}
							case 0x0065: {
								for (int i = 0; i <= x; ++i) {
									V[i] = (byte)(memory[I + i]);
								}
								//I += (byte)(x + 1);
								break;
							}
							case 0x0075: {
								if (x > 7) x = 7;
								for (int i = 0; i <= x; ++i) {
									SV[i] = V[i];
								}
								break;
							}
							case 0x0085: {
								if (x > 7) x = 7;
								for (int i = 0; i <= x; ++i) {
									V[i] = SV[i];
								}
								break;
							}
						}
						break;
					}
				default:
					break;
			}
		}
		if (sound_timer > 0) {
			if (sound_timer == 1) {
				GetComponent<AudioSource>().Play();
			}
			--sound_timer;
		}
		if (delay_timer > 0) {
			--delay_timer;
		}
	}

	void exitGame() {

	}
}
