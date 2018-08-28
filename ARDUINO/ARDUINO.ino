int lastLever = 0;
int lever = 0;

int buttonPin = 10;
int currentButtonState = 1;
int lastButtonState = 1;
int buttonHit = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  pinMode(buttonPin, INPUT_PULLUP);
}
//139, 645
void loop() {
  // smooth as fuck
  int currentLever = analogRead(0);

  lever = (lastLever * .9) + (currentLever * .1);

  lastLever = lever;

  buttonHit = 0;
  
  currentButtonState = digitalRead(buttonPin);
  if(currentButtonState != lastButtonState && currentButtonState == 0) {
    buttonHit = 1;
  }
  lastButtonState = currentButtonState;
  
  // string should come out like lever,buttonState as strings
  Serial.print(lever);
  Serial.print(",");
  Serial.println(buttonHit);

  delay(20);
}
