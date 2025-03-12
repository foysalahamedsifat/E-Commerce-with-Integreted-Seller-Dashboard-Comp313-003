import { createSlice } from "@reduxjs/toolkit";

const initialState = [];

const cartSlice = createSlice({
  name: "cart",
  initialState,
  reducers: {
    addToCart: (state, action) => {
      const product = action.payload;
      const existingProduct = state.find((item) => item.productId === product.productId);

      if (existingProduct) {
        existingProduct.quantity += 1;
      } else {
        state.push({ ...product, quantity: 1 });
      }
    },

    removeFromCart: (state, action) => {
      const productId = action.payload;
      return state.filter((item) => item.productId !== productId);
    },

    decreaseQuantity: (state, action) => {
      const productId = action.payload;
      const existingProduct = state.find((item) => item.productId === productId);

      if (existingProduct) {
        if (existingProduct.quantity > 1) {
          existingProduct.quantity -= 1;
        } else {
          // Remove if quantity drops to zero
          return state.filter((item) => item.productId !== productId);
        }
      }

      return state;
    },

    clearCart: (state) => {
      // Mutate the existing state instead of returning a new one
      state.length = 0;
    },
  },
});

export const { addToCart, removeFromCart, decreaseQuantity, clearCart } = cartSlice.actions;

export default cartSlice.reducer;
