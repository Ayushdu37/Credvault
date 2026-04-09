import { Component, ElementRef, ViewChild, AfterViewInit, OnDestroy, NgZone, HostListener } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import * as THREE from 'three';

@Component({
  standalone: true,
  selector: 'app-landing',
  imports: [RouterLink, LucideAngularModule],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css'
})
export class LandingComponent implements AfterViewInit, OnDestroy {
  @ViewChild('threejsContainer', { static: true }) containerRef!: ElementRef;
  
  private renderer!: THREE.WebGLRenderer;
  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private cardMesh!: THREE.Mesh;
  private cardGroup!: THREE.Group;
  private animationId!: number;
  private resizeListener!: () => void;
  
  // Mouse interaction
  private targetRotation = { x: 0, y: 0 };
  private currentRotation = { x: 0, y: 0 };
  
  constructor(private ngZone: NgZone) {}

  ngAfterViewInit(): void {
    this.initThreeJs();
  }

  ngOnDestroy(): void {
    if (this.animationId) {
      cancelAnimationFrame(this.animationId);
    }
    if (this.resizeListener) {
      window.removeEventListener('resize', this.resizeListener);
    }
    if (this.renderer) {
      this.renderer.dispose();
      this.containerRef.nativeElement.removeChild(this.renderer.domElement);
    }
  }

  onMouseMove(event: MouseEvent): void {
    const container = this.containerRef.nativeElement as HTMLElement;
    const rect = container.getBoundingClientRect();
    
    // Calculate normalized mouse coordinates (-1 to +1)
    const x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    const y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
    
    // Map to rotation limits
    this.targetRotation.y = x * 0.5; // Max 0.5 radians horizontally
    this.targetRotation.x = y * 0.3; // Max 0.3 radians vertically
  }

  onMouseLeave(): void {
    this.targetRotation.x = 0;
    this.targetRotation.y = 0;
  }

  private createCardTexture(): THREE.CanvasTexture {
    const canvas = document.createElement('canvas');
    canvas.width = 1024;
    canvas.height = 640;
    const ctx = canvas.getContext('2d')!;

    // Background gradient
    const gradient = ctx.createLinearGradient(0, 0, 1024, 640);
    gradient.addColorStop(0, '#2a2a2a');
    gradient.addColorStop(1, '#0a0a0a');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, 1024, 640);

    // Subtle tech lines
    ctx.strokeStyle = '#333333';
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.moveTo(0, 200); ctx.lineTo(1024, 200);
    ctx.moveTo(0, 400); ctx.lineTo(1024, 400);
    ctx.stroke();

    // Text: CredVault
    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 70px Arial, sans-serif';
    ctx.fillText('CredVault', 80, 120);

    // Text: Type
    ctx.fillStyle = '#888888';
    ctx.font = '30px Arial, sans-serif';
    ctx.fillText('C O R P O R A T E', 80, 170);

    // Draw EMV Chip outline
    ctx.strokeStyle = '#ddaa44';
    ctx.lineWidth = 4;
    ctx.strokeRect(100, 250, 120, 90);
    ctx.strokeRect(120, 270, 80, 50);

    // Card Number
    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 55px Courier New, monospace';
    ctx.fillText('4000  1234  5678  9010', 80, 480);

    // Expiry and Name
    ctx.font = '35px Arial, sans-serif';
    ctx.fillStyle = '#cccccc';
    ctx.fillText('EXP 12/28', 80, 550);
    ctx.fillText('ALEXANDER NODE', 400, 550);

    // Red Brand Orb
    ctx.fillStyle = '#dc2626';
    ctx.beginPath();
    ctx.arc(880, 500, 60, 0, Math.PI * 2);
    ctx.fill();

    const texture = new THREE.CanvasTexture(canvas);
    return texture;
  }

  private createCardBackTexture(): THREE.CanvasTexture {
    const canvas = document.createElement('canvas');
    canvas.width = 1024;
    canvas.height = 640;
    const ctx = canvas.getContext('2d')!;

    // Background gradient
    const gradient = ctx.createLinearGradient(0, 0, 1024, 640);
    gradient.addColorStop(0, '#1a1a1a');
    gradient.addColorStop(1, '#050505');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, 1024, 640);

    // Magnetic Stripe
    ctx.fillStyle = '#000000';
    ctx.fillRect(0, 80, 1024, 120);

    // Signature Panel
    const sigGradient = ctx.createLinearGradient(60, 260, 760, 260);
    sigGradient.addColorStop(0, '#f0f0f0');
    sigGradient.addColorStop(1, '#cccccc');
    ctx.fillStyle = sigGradient;
    ctx.fillRect(60, 260, 700, 80);

    // Text: CVV
    ctx.fillStyle = '#111111';
    ctx.font = 'italic 35px Courier New, monospace';
    ctx.fillText('942', 780, 315);

    // Magnetic Stripe reflection illusion
    ctx.fillStyle = 'rgba(255,255,255,0.05)';
    ctx.fillRect(0, 120, 1024, 10);

    // Fine print text
    ctx.fillStyle = '#555555';
    ctx.font = '20px Arial, sans-serif';
    ctx.fillText('This card is issued by CredVault Bank pursuant to a license from Mastercard International.', 60, 480);
    ctx.fillText('If found, please return to: CredVault, 100 Financial District, New York, NY 10005.', 60, 520);
    ctx.fillText('Use of this card constitutes acceptance of the terms and conditions.', 60, 560);

    // Small logo on back
    ctx.fillStyle = '#333333';
    ctx.font = 'bold 30px Arial, sans-serif';
    ctx.fillText('CredVault', 820, 560);

    const texture = new THREE.CanvasTexture(canvas);
    // Since this is placed on the "back" face of a ThreeJS BoxGeometry,
    // we need to horizontally flip the texture so it renders the correct way reading left-to-right
    texture.wrapS = THREE.RepeatWrapping;
    texture.repeat.x = -1;
    return texture;
  }

  private initThreeJs(): void {
    const container = this.containerRef.nativeElement as HTMLElement;

    // Scene setup
    this.scene = new THREE.Scene();

    // Camera setup
    this.camera = new THREE.PerspectiveCamera(45, container.clientWidth / container.clientHeight, 0.1, 100);
    this.camera.position.z = 10;

    // Renderer setup
    this.renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    this.renderer.setSize(container.clientWidth, container.clientHeight);
    this.renderer.setPixelRatio(window.devicePixelRatio);
    container.appendChild(this.renderer.domElement);

    // Group to hold the card and allow separate wobble/mouse rot logic
    this.cardGroup = new THREE.Group();
    this.scene.add(this.cardGroup);

    // Geometry: Credit Card Shape
    const geometry = new THREE.BoxGeometry(5.4, 3.4, 0.05); // Thinner depth
    
    // Generate Canvas Texture
    const cardTexture = this.createCardTexture();
    cardTexture.anisotropy = this.renderer.capabilities.getMaxAnisotropy();

    // Material: Premium Glass/Metal Hybrid (FRONT)
    const materialFront = new THREE.MeshPhysicalMaterial({
      map: cardTexture,
      color: 0xffffff,
      metalness: 0.6,
      roughness: 0.3,
      clearcoat: 1.0,
      clearcoatRoughness: 0.1,
    });

    // Generate Card Back Texture
    const backTexture = this.createCardBackTexture();
    backTexture.anisotropy = this.renderer.capabilities.getMaxAnisotropy();

    // Material: Premium Glass/Metal Hybrid (BACK)
    const materialBack = new THREE.MeshPhysicalMaterial({
      map: backTexture,
      color: 0xffffff,
      metalness: 0.6,
      roughness: 0.3,
      clearcoat: 1.0,
      clearcoatRoughness: 0.1,
    });

    // For the edges, we can use a standard dark material
    const edgeMaterial = new THREE.MeshPhysicalMaterial({ color: 0x111111, metalness: 0.8, roughness: 0.2 });
    
    // Apply materials [right, left, top, bottom, front, back]
    this.cardMesh = new THREE.Mesh(geometry, [edgeMaterial, edgeMaterial, edgeMaterial, edgeMaterial, materialFront, materialBack]);
    
    // Rotate the card to stand upright
    this.cardMesh.rotation.set(-0.2, 0.5, 0.1);
    this.cardGroup.add(this.cardMesh);

    // Lighting
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.8);
    this.scene.add(ambientLight);

    const dirLight1 = new THREE.DirectionalLight(0xffffff, 2);
    dirLight1.position.set(5, 5, 5);
    this.scene.add(dirLight1);

    const dirLight2 = new THREE.DirectionalLight(0xdc2626, 3); // Accent red
    dirLight2.position.set(-5, -5, 5);
    this.scene.add(dirLight2);

    const pointLight = new THREE.PointLight(0xffffff, 1, 20);
    pointLight.position.set(0, 2, 8);
    this.scene.add(pointLight);

    // Handle Resize falls cleanly on the loop now, but we keep window listener just in case
    this.resizeListener = () => {
       // logic moved to animation loop for safety against 0 height paints
    };
    window.addEventListener('resize', this.resizeListener);

    // Animation Loop
    this.ngZone.runOutsideAngular(() => {
      const animate = () => {
        this.animationId = requestAnimationFrame(animate);

        const time = Date.now() * 0.001;

        // Auto-Resize logic (fixes the 0x0 bug on spawn)
        const canvas = this.renderer.domElement;
        const pixelRatio = window.devicePixelRatio || 1;
        
        // Calculate the actual target buffer sizes
        const targetWidth = Math.floor(container.clientWidth * pixelRatio);
        const targetHeight = Math.floor(container.clientHeight * pixelRatio);
        
        // Only trigger a resize if the actual buffer dimensions don't match the target
        const needResize = canvas.width !== targetWidth || canvas.height !== targetHeight;
        
        if (container.clientWidth > 0 && container.clientHeight > 0 && needResize) {
          this.renderer.setSize(container.clientWidth, container.clientHeight, false);
          this.camera.aspect = container.clientWidth / container.clientHeight;
          this.camera.updateProjectionMatrix();
        }

        // Continuous floating
        this.cardGroup.position.y = Math.sin(time) * 0.15;
        // Continuous slow rotation if not interacting
        this.cardGroup.rotation.y += 0.002;

        // Mouse interaction interpolation (springy follow)
        this.currentRotation.x += (this.targetRotation.x - this.currentRotation.x) * 0.1;
        this.currentRotation.y += (this.targetRotation.y - this.currentRotation.y) * 0.1;

        // Apply mouse tilt to the mesh (on top of the group's continuous rotation)
        this.cardMesh.rotation.x = -0.2 + this.currentRotation.x;
        // Mix standard rotation with mouse tilt
        this.cardMesh.rotation.y = 0.5 + this.currentRotation.y;

        this.renderer.render(this.scene, this.camera);
      };
      animate();
    });
  }
}
