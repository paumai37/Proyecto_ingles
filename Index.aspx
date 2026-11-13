<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="ProyectoEnglish.WebForm6" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Healthcare Centre | Home</title>
    <link rel="stylesheet" href="css\StyleSheet1.css">
</head>
    
<body>
 
  
  <header id="home">
    <img src="images/logo.jpg" alt="Healthcare Centre logo" width="80">
    <h1>Healthcare Centre</h1>
    <p>Your health, our priority</p>

    <nav>
      <ul>
        <li><a href="#home">Home</a></li>
        <li><a href="#about">About us</a></li>
        <li><a href="#services">Services</a></li>
        <li><a href="#gallery">Gallery</a></li>
        <li><a href="#contact">Contact</a></li>
        <li><a href="Login.aspx">Log in</a></li>
      </ul>
    </nav>
  </header>
 
  <main>
 
    
    <section>
      <img src="images/hero.png" alt="Healthcare Centre main building" width="100%">
      <h2>Welcome to our healthcare centre</h2>
      <p>Professional and caring medical attention for the whole family, with modern facilities and an experienced team.</p>
      <p><a href="#contact">Book an appointment</a></p>
    </section>
 
    <hr>
 
    
    <section id="about">
      <h2>About our centre</h2>
      <img src="images/about.jpg" alt="Our healthcare centre" width="500">
      <h3>Who we are</h3>
      <p>We are a healthcare centre dedicated to the prevention, diagnosis and treatment of illnesses, offering a warm and professional environment for every patient.</p>
      <p>Our team of doctors, nurses and specialists works together to guarantee personalised care, using the latest technology and always putting patients first.</p>
      <p><strong>Opening hours:</strong> Monday to Friday, 8:00 – 20:00.</p>
    </section>
 
    <hr>
 
   
    <section id="services">
      <h2>Our services</h2>
      <p>A wide range of medical services under one roof.</p>
 
      <article>
        <img src="images/service1.jpg" alt="General medicine" width="250">
        <h3>General medicine</h3>
        <p>Check-ups, diagnosis and follow-up of your health at any age.</p>
      </article>
 
      <article>
        <img src="images/service2.jpg" alt="Paediatrics" width="250">
        <h3>Paediatrics</h3>
        <p>Specialised care for babies, children and teenagers.</p>
      </article>
 
      <article>
        <img src="images/service3.jpg" alt="Cardiology" width="250">
        <h3>Cardiology</h3>
        <p>Heart health tests, monitoring and personalised treatments.</p>
      </article>
 
      <article>
        <img src="images/service4.jpg" alt="Laboratory" width="250">
        <h3>Laboratory</h3>
        <p>Fast and reliable blood tests and clinical analysis.</p>
      </article>
    </section>
 
    <hr>
 
    
    <section id="gallery">
      <h2>Our facilities</h2>
      <p>Take a look at our modern and comfortable spaces.</p>
      <img src="images/gallery1.jpg" alt="Reception" width="300">
      <img src="images/gallery2.jpg" alt="Waiting room" width="300">
      <img src="images/gallery3.jpg" alt="Consultation room" width="300">
    </section>
 
    <hr>
 
    
    <section id="contact">
      <h2>Contact us</h2>
      <p>We are here to help. Get in touch or visit us.</p>
 
      <h3>Address</h3>
      <p>Street Name 123<br>46000 Valencia, Spain</p>
 
      <h3>Phone</h3>
      <p>+34 900 000 000</p>
 
      <h3>Email</h3>
      <p><a href="mailto:info@healthcarecentre.com">info@healthcarecentre.com</a></p>
    </section>
 
  </main>
 
  <hr>
 
  
  <footer>
    <h4>Healthcare Centre</h4>
    <p>Quality medical care for you and your family.</p>
 
    <h4>Quick links</h4>
    <ul>
      <li><a href="#home">Home</a></li>
      <li><a href="#about">About us</a></li>
      <li><a href="#services">Services</a></li>
      <li><a href="#contact">Contact</a></li>
    </ul>
 
    <h4>Opening hours</h4>
    <ul>
      <li>Mon – Fri: 8:00 – 20:00</li>
      <li>Saturday: 9:00 – 14:00</li>
      <li>Sunday: Closed</li>
    </ul>
 
    <p> 2026 Healthcare Centre. All rights reserved.</p>
  </footer>
 
</body>
</html>
