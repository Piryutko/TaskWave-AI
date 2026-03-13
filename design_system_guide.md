# ToneWave UI Design System Guide 🌊

This guide outlines the core design principles and code tokens used to create the premium "ToneWave" aesthetic. You can follow these steps to replicate the style using any other color palette.

---

## 1. Core Color Architecture

The "Wave" style relies on a **Deep Dark Base** combined with a **Single Vibrant Accent**.

### Current Palette (Teal)
*   **Base Background**: `#0A120E` (Deep Black-Green)
*   **Primary Accent**: `#2DD4BF` (Teal-400)
*   **Accent Glow**: `rgba(45, 212, 191, 0.3)`
*   **Text (Primary)**: `teal-50` (Very bright off-white)
*   **Text (Secondary)**: `white/30` or `white/40`

### 🎨 How to Swap Colors
To maintain the same vibe with a different color (e.g., Purple), replace the tokens:
1.  **Accent**: Switch `#2DD4BF` to `#A855F7` (Purple-500).
2.  **Background Tint**: Change the background from `#0A120E` (Green tint) to `#0F0A12` (Purple tint).
3.  **Glows**: Replace `bg-teal-900/20` with `bg-purple-900/20`.

---

## 2. Glassmorphism Formula

The signature "Frosted Glass" look is achieved through this specific Tailwinds stack:

```html
<!-- Glass Card Example -->
<div class="bg-white/5 border border-white/10 backdrop-blur-xl rounded-[32px] shadow-inner shadow-white/5">
    Content
</div>
```

*   **Transparency**: Never use solid backgrounds. Use `white/5` or `white/[0.03]`.
*   **Borders**: Use `white/10` for card borders and `white/5` for inner dividers.
*   **Blur**: `backdrop-blur-xl` or `backdrop-blur-2xl` is essential for the premium feel.
*   **Inner Depth**: `shadow-inner shadow-white/5` adds a subtle 3D highlight.

---

## 3. Atmospheric Glows

Add "life" to the background using blurred, pulsing circles:

```html
<div class="fixed inset-0 pointer-events-none">
    <div class="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-teal-900/20 rounded-full blur-[120px] animate-pulse"></div>
    <div class="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] bg-teal-800/10 rounded-full blur-[120px] animate-pulse"></div>
</div>
```

---

## 4. Typography & Accents

*   **Headers**: `font-extrabold tracking-tight` with a subtle gradient (`from-white to-accent`).
*   **Labels/Stats**: `font-black uppercase tracking-[0.3em] text-[10px]`. This "Laconic" style adds an engineering/premium feel.
*   **Active States**: High contrast. Use `bg-accent` with `text-dark-base`.
*   **Micro-indicators**: 
    ```html
    <span class="w-2.5 h-2.5 rounded-full bg-accent animate-pulse shadow-[0_0_12px_accent]"></span>
    ```

---

---

## 5. Landing Page Anatomy

The landing page follows a "Clean & Bold" hierarchy designed to draw focus to the CTA.

### The "App Status" Badge
Use this for non-urgent announcements or version labels:
```html
<div class="inline-flex items-center gap-3 px-5 py-2.5 rounded-[32px] bg-white/5 border border-white/10 backdrop-blur-xl saturate-150">
    <span class="w-2.5 h-2.5 rounded-full bg-accent animate-pulse shadow-[0_0_12px_accent]"></span>
    <span class="text-sm font-medium tracking-wide text-white/80 uppercase">Communication Editor</span>
</div>
```

### Hero Title Strategy
*   **Size**: `text-8xl` (Desktop) to create immediate impact.
*   **Gradients**: Use `bg-clip-text text-transparent` with a `bg-gradient-to-br`.
*   **Secondary Text**: Use a distinct accent color for the last word (e.g., `from-accent to-accent-dark`).

### The Shimmering CTA
A premium button that feels interactive:
*   **Shape**: `rounded-[32px]` (Extra round).
*   **Hover**: `hover:shadow-[0_0_40px_rgba(accent,0.2)] hover:border-accent/50`.
*   **Shimmer Effect**: An absolute-positioned gradient div that slides across the button on hover.

---

## 6. Advanced Typography

| Element | Style | Tailwinds Classes |
| :--- | :--- | :--- |
| **Main Title** | Extra Bold, Tight | `text-8xl font-extrabold tracking-tight` |
| **Section Header**| Black, Uppercase | `text-base font-black uppercase tracking-widest` |
| **Micro-Caps** | Small, Wide Tracking| `text-[10px] uppercase tracking-[0.4em] font-medium` |
| **Body Text** | Medium, Relaxed | `text-lg text-white/60 font-medium leading-relaxed` |

---

## 7. Layout & Positioning

*   **Z-Index Layers**: 
    *   `z-0`: Background glows.
    *   `z-10`: Landing content.
    *   `z-20/50`: App interface and modals.
*   **Centering**: Use `flex flex-col items-center justify-center text-center` for hero sections to ensure balance.
*   **Max-Widths**: 
    *   Landing: `max-w-5xl` for text focus.
    *   App: `max-w-[1700px]` for high-utility workspaces.

---

## 8. Summary Checklist for New Projects

1.  [ ] **Theme**: Pick 1 primary accent color (high saturation).
2.  [ ] **Base**: Set background to a near-black version of that accent.
3.  [ ] **Layers**: Use the Glass Formula for all containers.
4.  [ ] **Light**: Place 2-3 large, low-opacity blurred glows in corners.
5.  [ ] **Details**: Add tracking-widest uppercase labels for all metrics.
6.  [ ] **Motion**: Use `animate-pulse` for status lights and `transition-all` for all hovers.
