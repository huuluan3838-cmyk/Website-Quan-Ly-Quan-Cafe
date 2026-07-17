using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOAN_QUANCAFE;

namespace DOAN_QUANCAFE.Controllers
{
    [PhanQuyenAdmin]
    public class LoaiSanPhamsController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: LoaiSanPhams
        public ActionResult Index()
        {
            return View(db.tblLoaiSanPhams.ToList());
        }

        // GET: LoaiSanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoaiSanPham tblLoaiSanPham = db.tblLoaiSanPhams.Find(id);
            if (tblLoaiSanPham == null)
            {
                return HttpNotFound();
            }
            return View(tblLoaiSanPham);
        }

        // GET: LoaiSanPhams/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoaiSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLoai,TenLoai,GhiChu")] tblLoaiSanPham tblLoaiSanPham)
        {
            if (ModelState.IsValid)
            {
                db.tblLoaiSanPhams.Add(tblLoaiSanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblLoaiSanPham);
        }

        // GET: LoaiSanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoaiSanPham tblLoaiSanPham = db.tblLoaiSanPhams.Find(id);
            if (tblLoaiSanPham == null)
            {
                return HttpNotFound();
            }
            return View(tblLoaiSanPham);
        }

        // POST: LoaiSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoai,TenLoai,GhiChu")] tblLoaiSanPham tblLoaiSanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblLoaiSanPham).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblLoaiSanPham);
        }

        // GET: LoaiSanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoaiSanPham tblLoaiSanPham = db.tblLoaiSanPhams.Find(id);
            if (tblLoaiSanPham == null)
            {
                return HttpNotFound();
            }
            return View(tblLoaiSanPham);
        }

        // POST: LoaiSanPhams/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    tblLoaiSanPham tblLoaiSanPham = db.tblLoaiSanPhams.Find(id);
        //    db.tblLoaiSanPhams.Remove(tblLoaiSanPham);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // 1️⃣ Lấy loại sản phẩm
            var loai = db.tblLoaiSanPhams.Find(id);

            // 2️⃣ Lấy danh sách sản phẩm thuộc loại
            var dsSanPham = db.tblSanPhams
                              .Where(sp => sp.LoaiSP == id)
                              .ToList();

            foreach (var sp in dsSanPham)
            {
                // 3️⃣ XÓA HÌNH ẢNH CỦA SẢN PHẨM
                var dsHinhAnh = db.tblHinhAnhs
                                  .Where(ha => ha.MaSP == sp.MaSP)
                                  .ToList();

                db.tblHinhAnhs.RemoveRange(dsHinhAnh);

                // 4️⃣ XÓA BÌNH LUẬN CỦA SẢN PHẨM
                var dsBinhLuan = db.BinhLuans
                                   .Where(bl => bl.MaSP == sp.MaSP)
                                   .ToList();

                db.BinhLuans.RemoveRange(dsBinhLuan);

                // 5️⃣ XÓA SẢN PHẨM
                db.tblSanPhams.Remove(sp);
            }

            // 6️⃣ XÓA LOẠI SẢN PHẨM
            db.tblLoaiSanPhams.Remove(loai);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
