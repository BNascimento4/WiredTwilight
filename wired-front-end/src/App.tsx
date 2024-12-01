import React from "react";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import CreatePost from "./forum/CriarPost";
import CreateForum from "./forum/forum";
import ForumList from "./forum/forums";
import Login from "./login/login";
import Registro from "./registro/registro";
import AtualizarUsername from "./usuario/atualizar-username";

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/registro" element={<Registro />} />
        <Route path="/posts" element={<ForumList />} />
        <Route path="/forums" element={<ForumList />} />
        <Route path="/forum/:forumId" element={<CreatePost />} />
        <Route path="/forum/criar" element={<CreateForum />} />
        <Route path="/forum/:forumId/posts" element={<CreatePost />} />
        <Route path="/forum/:forumId/post" element={<CreatePost />} />
        <Route path="/atualizar-username" element={<AtualizarUsername />} /> {/* Nova rota */}
      </Routes>
    </Router>
  );
};

export default App;
