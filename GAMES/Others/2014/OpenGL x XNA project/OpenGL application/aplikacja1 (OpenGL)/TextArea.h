#ifndef __TEXTAREA_H
#define __TEXTAREA_H

#include <windows.h>		// Header File For Windows
#include <gl\gl.h>			// Header File For The OpenGL32 Library
#include <gl\glu.h>			// Header File For The GLu32 Library
#include <gl\glaux.h>		// Header File For The Glaux Library
#include <gl\glut.h>
#include <math.h>
#include <stdio.h>
#include "Fps.h"
#include "TimeCounter.h"

class TextArea
{
	FPS * fps1;
	TimeCounter * time1;

private:
	float oddalenie0;
	float maxWindow;

public:
	TextArea(FPS * fps1, TimeCounter * time1);

private:
	int InitGL0(GLvoid);

	void drawString0 (void * font, char * s, float x, float y, float z);

public:
	int DrawGLScene(int tryb, bool timePause);
};

#endif