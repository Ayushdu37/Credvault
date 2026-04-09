import { Component, inject, ViewChild, ElementRef, AfterViewInit, OnDestroy, NgZone } from '@angular/core';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { AsyncPipe, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import * as THREE from 'three';
import { AuthActions } from '../../../store/auth/auth.actions';
import {
  selectAuthLoading,
  selectAuthError,
} from '../../../store/auth/auth.selectors';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [FormsModule, AsyncPipe, RouterLink, LucideAngularModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements AfterViewInit, OnDestroy {
  private store = inject(Store);
  private ngZone = inject(NgZone);
  
  @ViewChild('threejsContainer', { static: true }) containerRef!: ElementRef;
  private renderer!: THREE.WebGLRenderer;
  private animationId!: number;
  private resizeListener!: () => void;
  
  email = '';
  password = '';
  
  loading$ = this.store.select(selectAuthLoading);
  error$ = this.store.select(selectAuthError);
  
  ngAfterViewInit(): void {
    this.initThreeJs();
  }

  ngOnDestroy(): void {
    if (this.animationId) {
      cancelAnimationFrame(this.animationId);
    }
    if (this.renderer) {
      this.renderer.dispose();
    }
    if (this.resizeListener) {
      window.removeEventListener('resize', this.resizeListener);
    }
  }

  private initThreeJs(): void {
    this.ngZone.runOutsideAngular(() => {
      const container = this.containerRef.nativeElement;
      
      // Setup scene
      const scene = new THREE.Scene();
      
      // Setup camera (aspect will be updated on resize, default to 1 for now)
      const camera = new THREE.PerspectiveCamera(45, 1, 0.1, 1000);
      camera.position.z = 6;

      // Setup renderer
      this.renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
      this.renderer.setPixelRatio(window.devicePixelRatio);
      container.appendChild(this.renderer.domElement);

      // Create a premium Charcoal + Red Torus Knot
      const geometry = new THREE.TorusKnotGeometry(1.4, 0.4, 128, 32);
      
      // Core solid dark metallic material
      const solidMaterial = new THREE.MeshStandardMaterial({
        color: 0x0a0a0a,
        metalness: 0.9,
        roughness: 0.1,
      });

      const mesh = new THREE.Mesh(geometry, solidMaterial);
      
      // Wireframe overlay for that technical/fintech look
      const wireframeGeometry = new THREE.WireframeGeometry(geometry);
      const wireframeMaterial = new THREE.LineBasicMaterial({ 
        color: 0xef4444, // brand red
        transparent: true, 
        opacity: 0.3 
      });
      const wireframe = new THREE.LineSegments(wireframeGeometry, wireframeMaterial);
      mesh.add(wireframe);

      scene.add(mesh);

      // Lighting
      const ambientLight = new THREE.AmbientLight(0xffffff, 0.3);
      scene.add(ambientLight);

      // Deep red dramatic rim light
      const redLight = new THREE.DirectionalLight(0xef4444, 4);
      redLight.position.set(5, 5, 2);
      scene.add(redLight);

      // Subtle cool fill light
      const fillLight = new THREE.DirectionalLight(0x3b82f6, 1.5);
      fillLight.position.set(-5, -5, -2);
      scene.add(fillLight);

      // Handle Resize properly
      const resize = () => {
        const width = container.clientWidth;
        const height = container.clientHeight;
        if (width === 0 || height === 0) return; // ignore if hidden
        
        camera.aspect = width / height;
        camera.updateProjectionMatrix();
        this.renderer.setSize(width, height);
      };
      
      this.resizeListener = resize;
      window.addEventListener('resize', this.resizeListener);
      
      // Initial sizing
      setTimeout(resize, 0);

      const animate = () => {
        this.animationId = requestAnimationFrame(animate);
        // Premium slow rotation
        mesh.rotation.x += 0.002;
        mesh.rotation.y += 0.004;
        mesh.rotation.z += 0.001;
        this.renderer.render(scene, camera);
      };

      animate();
    });
  }

  onLogin(): void {
    if (this.email && this.password) {
      this.store.dispatch(
        AuthActions.login({ email: this.email, password: this.password })
      );
    }
  }
}