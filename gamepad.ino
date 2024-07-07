const int buttonPin = 5;     // pin nối button để điều khiển right => white line
const int buttonPin2 = 6;     // pin nối button để điều khiển left => green line
const int buttonPin3 = 7;     // pin nối button để điều khiển shoot => red line
const int ledPin =  13;      // pin nối LED

// Tạo một biến nhận diện trạng thái button:
int buttonState = 0;         
int buttonState2 = 0;        
int buttonState3 = 0;        

void setup() {
  // set ledPin là output
  pinMode(LED_BUILTIN, OUTPUT);
  // set buttonPin là input để đọc giá trị từ button
  pinMode(buttonPin, INPUT);
  pinMode(buttonPin2, INPUT);
  pinMode(buttonPin3, INPUT);
  Serial.begin(9600);
}

void loop() {
  // đọc giá trị button rồi lưu vào buttonState
  buttonState = digitalRead(buttonPin);
  buttonState2 = digitalRead(buttonPin2);
  buttonState3 = digitalRead(buttonPin3);

  // nếu button được nhấn, buttonState nhận giá trị HIGH, và ngược lại
  if (buttonState == HIGH) {
    // Bật LED:
    digitalWrite(LED_BUILTIN, HIGH);
    Serial.print("R");
    //delay(100);
  } 
  if (buttonState2 == HIGH) {
    // Bật LED:
    digitalWrite(LED_BUILTIN, HIGH);
    Serial.print("L");
  } 
  if (buttonState3 == HIGH) {
    // Bật LED:
    digitalWrite(LED_BUILTIN, HIGH);
    Serial.print("F");
    //delay(100);
  } 
  if (buttonState3 == LOW && buttonState == LOW && buttonState2 == LOW) {
    // Tắt LED:
    Serial.print(" ");
    digitalWrite(LED_BUILTIN, LOW);
  }
}