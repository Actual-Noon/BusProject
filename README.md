# ตามล้อรถบัส (Follow the Bus)
A Data-Driven 3D Simulation Game for Behavioral Decision-Making Analysis under Time Constraints

<img width="1919" height="1079" alt="Screenshot 2026-10-03 101507" src="https://github.com/user-attachments/assets/808792a6-ebd5-4c95-813f-4aaeb0c38183" />

<img width="1506" height="654" alt="Screenshot 2026-10-03 101519" src="https://github.com/user-attachments/assets/0b2e50d7-2313-4d25-8b4f-6ecacb27d42f" />

---

## ภาพรวมโครงงาน (Overview)
ตามล้อรถบัส คือเกมจำลองสถานการณ์ 3 มิติเชิงรุก (Active Learning) ที่พัฒนาขึ้นด้วย Unity Engine และภาษา C# โดยมีเป้าหมายเพื่อศึกษากระบวนการตัดสินใจและการบริหารเวลาของผู้เรียนระดับมัธยมศึกษาตอนปลายเมื่อต้องเผชิญกับสถานการณ์กดดัน (การตกรถบัสไปโรงเรียน)

โครงงานนี้มุ่งเน้นการประยุกต์ใช้ทฤษฎีทางคอมพิวเตอร์และการพัฒนาซอฟต์แวร์ ทั้งเรื่อง Data Persistence, State Management และ Procedural Randomization เพื่อจัดเก็บและวิเคราะห์ข้อมูลสถิติพฤติกรรมของผู้เล่นอย่างเป็นระบบ

---

## ฟีเจอร์หลัก (Key Features)

### ระบบการเล่น (Gameplay Mechanics)
- Diegetic "No-Pause" Phone Menu: การกด ESC เพื่อดึงหน้าจอมือถือขึ้นมาเป็นเมนูหลัก โดยที่เวลานับถอยหลังในเกมยังคงดำเนินอยู่จริง (Time.timeScale = 1) เพื่อจำลองสถานการณ์ความกดดันด้านเวลาแบบ Real-time
- Multiple Pathways & Vehicles: รองรับทางเลือกการแก้ปัญหาที่หลากหลาย เช่น การเดินหรือวิ่ง, การตามหาคีมตัดลวดเพื่อปลดล็อกจักรยาน และการปฏิสัมพันธ์กับ NPC วินมอเตอร์ไซค์
- Risk & Reward Obstacles: ระบบสิ่งกีดขวาง (รถยนต์ขับผ่าน) หากชนจะตัดเข้าสู่ฉาก Game Over ทันที
- Stamina & Sprint System: ระบบวิ่งเร็วพร้อม Exhaustion Screen Overlay เพื่อบีบให้ผู้เล่นต้องวางแผนบริหารพลังงาน

### จุดเน้นเชิงเทคนิค (Technical & CS Highlights)
- Real-time Data Tracking System: ระบบบันทึกสถิติการเล่นในแต่ละรอบ ทั้งเวลาที่ใช้, จำนวนเหรียญที่เก็บได้ และการกดตอบโต้กับวัตถุต่างๆ โดยแสดงผลผ่าน End-Game Analytics Dashboard
- Data Persistence (Save Run System): ระบบบันทึกข้อมูลการเล่นลงเครื่อง และเปรียบเทียบหา Best Time เพื่อนำไปแสดงผลย้อนหลังบนหน้า Main Menu
- Procedural Item Spawner: ระบบสุ่มตำแหน่งและโอกาสเกิด (Spawn Chance) ของไอเทมสำคัญในแต่ละ Scene เพื่อลดการจำ Pattern ของผู้เล่น ทำให้ได้ข้อมูลพฤติกรรมที่แม่นยำและไม่ลำเอียง (Unbiased Data)
- Responsive UI Canvas: การจัดการ UI ด้วยการล็อก Anchors เพื่อรองรับการแสดงผลทุกอัตราส่วนหน้าจอ

---

## เทคโนโลยีที่ใช้และสถาปัตยกรรม (Tech Stack & Architecture)

- Game Engine: Unity
- Programming Language: C# (.NET)
- Design Patterns:
  - Singleton Pattern (ใช้กับ Game Manager และ Data Tracker)
  - Component-Based Architecture
  - Parent-Child Entity Pattern (ใช้กับระบบ Item Spawner)
- Data Storage: PlayerPrefs / Local Data Persistence

---

## สถาปัตยกรรมระบบ (System Architecture)

[ Player Action ] ---> [ Interaction System ] ---> [ GameStatsManager (Singleton) ] ---> [ Main Menu Dashboard ] ---> [ Local Data Persistence ] ---> [ End-Game Analytics ]

---

## วิธีการติดตั้งและรันโปรเจกต์ (How to Run & Play)

### สำหรับผู้ทดลองเล่น / กรรมการประเมิน
1. ไปที่หัวข้อ Releases ด้านขวามือของ Repository
2. ดาวน์โหลดไฟล์ .zip ของตัวเกมเวอร์ชันล่าสุด
3. แตกไฟล์แล้วเปิดรันไฟล์ FollowTheBus.exe เพื่อเข้าเล่นได้ทันที

### สำหรับนักพัฒนา (Open in Unity)
1. Clone Repository นี้ลงเครื่องด้วยคำสั่ง:
   git clone [https://github.com/your-username/follow-the-bus.git](https://github.com/Actual-Noon/BusProject.git)
2. เปิดโปรแกรม Unity Hub เลือก Add project from disk
3. เลือกโฟลเดอร์โปรเจกต์ และเปิดผ่าน Unity Editor
4. ไปที่โฟลเดอร์ Assets/Scenes/MainMenu.unity แล้วกด Play

---

## ข้อความชี้แจงด้านลิขสิทธิ์ (Disclaimer)
โมเดล 3D และ visual assets ทั้งหมดที่นำมาใช้ในการประกอบฉากและสร้างสภาพแวดล้อมภายในเกมนี้ เป็นแบบ free assets ที่ใช้งานได้เพื่อการศึกษาและการพัฒนาโครงงาน (Free for Educational / Non-Commercial Use) จากแหล่งดาวน์โหลดสาธารณะและ Unity Asset Store โดยระบบเกม Logic, การเขียนสคริปต์ C# และการออกแบบระบบการเล่นทั้งหมดถูกพัฒนาขึ้นโดยผู้ทำโครงงานเอง

---

## ผู้พัฒนา (Developer)
- ชื่อ-นามสกุล: ธนัทเทพ ภู่ทอง
- บทบาท: Solo Developer
- สถานศึกษา: โรงเรียนรัตนโกสินทร์สมโภชบางเขน
- สาขาที่มุ่งหมาย: Computer Science (CS)
- ช่องทางติดต่อ: tanatap.p2552@gmail.com / 082-648-2556 / Facebook: Tanattep Poothong
