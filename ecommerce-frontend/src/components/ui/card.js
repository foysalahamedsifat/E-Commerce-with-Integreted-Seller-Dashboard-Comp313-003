import React from "react";

export const Card = ({ children }) => (
  <div className="border p-4 rounded-lg shadow-md bg-white">
    {children}
  </div>
);

export const CardContent = ({ children }) => (
  <div className="p-2">{children}</div>
);
